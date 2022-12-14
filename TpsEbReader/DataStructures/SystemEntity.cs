using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TpsEbReader.DataStructures
{
    public class SystemEntity
    {
        public SystemEntity()
        {
            Parameters = new List<Parameter>();
        }
        public string Name { get; set; }
        public string BlockType { get; set; }
        public string EbSource { get; set; }
        public List<Parameter> Parameters { get; set; }
    }
}
