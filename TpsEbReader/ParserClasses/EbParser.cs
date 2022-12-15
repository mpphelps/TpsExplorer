using System.Formats.Asn1;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using TpsEbReader.DataStructures;

namespace TpsEbReader;

public class EbParser
{
    private string _ebFolderPath;
    private Dictionary<string, Point> _points = new();  // pointName, pointData
    private Dictionary<string, Box> _boxes = new(); // boxName, boxData
    private Logger _logger = new(ErrorLevel.FullDebug);

    public void ParseEb(string folderPath)
    {
        _ebFolderPath = folderPath;
        var directoryInfo = new DirectoryInfo(_ebFolderPath);
        if (!directoryInfo.Exists)
        {
            _logger.LogMessage(ErrorLevel.Warning, $"invalid folder path {_ebFolderPath}");
        }

        var files = directoryInfo.GetFiles("*.EB");
        foreach (var file in files)
        {
            using var sr = new StreamReader(file.FullName);
            var psr = new PeekableStreamReaderAdapter(sr);
            ParseEbFile(psr);
        }

        ResolvePointReferences();

    }
    public List<Box> GetBoxes()
    {
        var boxes = new List<Box>();
        foreach (var box in _boxes)
        {
            boxes.Add(box.Value);
        }
        return boxes;
    }

    public List<Point> GetPoints()
    {
        var points = new List<Point>();
        foreach (var point in _points)
        {
            points.Add(point.Value);
        }
        return points;
    }
    private void ResolvePointReferences()
    {
        var connectionType = PointReferenceType.P2PConnection;
        foreach (var point in _points)
        {
            foreach (var parameter in point.Value.Parameters)
            {
                var tagName = parameter.Value;
                string tagParameter = "N/A";
                // Once at PKGNAME param, any references found after are AM connections
                if (parameter.Field.Contains("PKGNAME")) connectionType = PointReferenceType.AMConnection;
                if (parameter.Value.Contains('.'))
                {
                    var tagData = parameter.Value.Split('.');
                    tagName = tagData[0];
                    tagParameter = tagData[1];
                }
                if (_points.ContainsKey(tagName))
                {
                    // if the parameter contains a point reference,
                    // then add that reference to the source of current point
                    // and to destination of referenced point
                    point.Value.Sources.Add(new(_points[tagName], tagParameter, connectionType));
                    _points[tagName].Destinations.Add(new(point.Value, parameter.Field, connectionType));
                }
            }
        }
    }
    private void ParseEbFile(PeekableStreamReaderAdapter psr)
    {
        var state = ParseState.StartOfEntity;
        SystemEntity entity = new SystemEntity();
        while (!psr.EndOfStream)
        {
            state = state switch
            {
                ParseState.StartOfEntity    => ParseStartOfEntity(psr, ref entity),
                ParseState.EntityType       => ParseEntityType(psr, entity),
                ParseState.EntityName       => ParseEntityName(psr, entity),
                ParseState.EntityParameters => ParseEntityParameters(psr, entity),
                ParseState.EndOfEntity      => ParseEndOfEntity(entity),
                ParseState.SkipSystemEntity => SkipToNextEntity(psr),
                _ => state
            };
        }
        // adds the last point in the file
        if (entity.Name != null)
            if (entity is Point)
                _points.Add(entity.Name, (Point)entity);
            else if (entity is Box)
                _boxes.Add(entity.Name, (Box)entity); 
    }
    private ParseState ParseStartOfEntity(PeekableStreamReaderAdapter psr, ref SystemEntity? entity)
    {
        var line = psr.ReadLine();
        if (line.Contains("SYSTEM ENTITY"))
        {
            entity = psr.PeekLine().Contains("BOX") ? new Box() : new Point();
            entity.EbSource = psr.FileName;
            return ParseState.EntityType;
        }
        _logger.LogMessage(ErrorLevel.Warning, $"Parser expected system entity and instead got: {line}\n" +
                                               $"Skipping to next system Entity\n" + 
                                               $"Line: {psr.Line} in {psr.FileName}\n");
        return ParseState.SkipSystemEntity;
    }
    private ParseState ParseEntityType(PeekableStreamReaderAdapter psr, SystemEntity entity)
    {
        var line = psr.ReadLine();
        if (line.Contains("&T"))
        {
            entity.BlockType = line.Substring(3).Trim();
            entity.EbSource = psr.FileName;
            return ParseState.EntityName;
        }

        _logger.LogMessage(ErrorLevel.Warning, $"Parser expected entity type and instead got: {line}\n" +
                                               $"Skipping to next system Entity\n" +
                                               $"Line: {psr.Line} in {psr.FileName}\n");
        return ParseState.SkipSystemEntity;
    }
    private ParseState ParseEntityName(PeekableStreamReaderAdapter psr, SystemEntity entity)
    {
        var line = psr.ReadLine();
        if (line.Contains("&N"))
        {
            var tagName = line.Substring(3).Trim();
            if (entity is Point && _points.ContainsKey(tagName))
            {
                _logger.LogMessage(ErrorLevel.Warning, $"Parser found duplicate point: {tagName}\n" +
                                                       $"Skipping to next system Entity\n" +
                                                       $"Line: {psr.Line} in {psr.FileName}\n");
                return ParseState.SkipSystemEntity;
            }
            if (entity is Box && _boxes.ContainsKey(tagName))
            {
                _logger.LogMessage(ErrorLevel.Warning, $"Parser found duplicate box: {tagName}\n" +
                                                       $"Skipping to next system Entity\n" +
                                                       $"Line: {psr.Line} in {psr.FileName}\n");
                return ParseState.SkipSystemEntity;
            }
            entity.Name = tagName;
            return ParseState.EntityParameters;
        }

        _logger.LogMessage(ErrorLevel.Warning, $"Parser expected entity name and instead got: {line}\n" +
                                               $"Skipping to next system Entity\n" +
                                               $"Line:{psr.Line} in {psr.FileName}\n");

        return ParseState.SkipSystemEntity;
    }
    private ParseState ParseEntityParameters(PeekableStreamReaderAdapter psr, SystemEntity entity)
    {
        if (psr.PeekLine().Contains("SYSTEM ENTITY"))
        {
            _logger.LogMessage(ErrorLevel.FullDebug, $"Parser completed parsing entity {entity.Name}");
            return ParseState.EndOfEntity;
        }

        var line = psr.ReadLine();
        if (line.Contains('='))
        {
            var data = line.Split('=');
            data[0] = data[0].Trim();
            data[1] = data[1].Replace("\"","");
            data[1] = data[1].Trim();
            if (ContainsOnlySpecifiedChars(new[]{'-'}, data[1])) data[1] = data[1].Replace("-","");
            var parameter = new Parameter(data[0], data[1]);
            entity.Parameters.Add(parameter);
        }
        else
        {
            _logger.LogMessage(ErrorLevel.Warning, $"Parser expected point parameter info and instead got: {line}\n" +
                                                   $"Skipping to next system Entity\n" +
                                                   $"Line: {psr.Line} in {psr.FileName}\n");
            return ParseState.SkipSystemEntity;
        }

        return ParseState.EntityParameters;
    }
    private ParseState ParseEndOfEntity(SystemEntity? entity)
    {
        if (entity is Point)
            _points.Add(entity.Name, (Point)entity);
        else if (entity is Box)
            _boxes.Add(entity.Name, (Box)entity);
        return ParseState.StartOfEntity;
    }
    private ParseState SkipToNextEntity(PeekableStreamReaderAdapter psr)
    {
        if (psr.PeekLine().Contains("SYSTEM ENTITY"))
        {
            _logger.LogMessage(ErrorLevel.FullDebug, $"Parser successfully skipped to next System Entity\n" +
                                                     $"Line: {psr.Line} in {psr.FileName}\n");
            return ParseState.StartOfEntity;
        }
        var line = psr.ReadLine();
        return ParseState.SkipSystemEntity;
    }
    private bool ContainsOnlySpecifiedChars(char[] c, string text)
    {
        foreach (var letter in text)
        {
            if (!c.Contains(letter)) return false;
        }
        return true;
    }

}