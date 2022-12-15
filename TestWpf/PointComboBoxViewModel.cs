using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TpsEbReader;

namespace TestWpf
{
    public class PointComboBoxViewModel
    {
        private EbParser _parser;

        public PointComboBoxViewModel()
        {
            _parser = new EbParser();
        }

        public Task ExecuteEbParserAsync(string ebFolderPath)
        {
            return Task.Run(() => _parser.ParseEb(ebFolderPath));
        }

        public List<Point> GetEbPoints()
        {
            var points = _parser.GetPoints();
            points = points.OrderBy(p => p.Name).ToList();
            return points;
        }
        public List<Box> GetEbBoxes()
        {
            var boxes = _parser.GetBoxes();
            boxes = boxes.OrderBy(p => p.Name).ToList();
            return boxes;
        }
    }
}
