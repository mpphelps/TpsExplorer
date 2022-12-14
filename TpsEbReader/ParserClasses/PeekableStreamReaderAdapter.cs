namespace TpsEbReader;

public class PeekableStreamReaderAdapter
{
    private StreamReader _underlying;
    private Queue<string> _bufferedLines;

    public bool EndOfStream => _underlying.EndOfStream;
    public string FileName => (_underlying.BaseStream as FileStream)?.Name;
    public int Line { get; private set; }

    public PeekableStreamReaderAdapter(StreamReader underlying)
    {
        _underlying = underlying;
        _bufferedLines = new Queue<string>();
        Line = 0;

    }

    public string PeekLine()
    {
        if (_bufferedLines.Count == 0)
        {
            string line = _underlying.ReadLine();
            if (line == null)
                return null;
            _bufferedLines.Enqueue(line);
        }
        return _bufferedLines.Peek();
    }
    public string ReadLine()
    {
        Line++;
        if (_bufferedLines.Count > 0)
            return _bufferedLines.Dequeue();
        return _underlying.ReadLine();
    }

    public void ReadToEnd()
    {
        _underlying.ReadToEnd();
    }
}