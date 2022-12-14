using System;
using System.IO;

namespace TpsEbReader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var ebFolderPath = @"C:\Users\h111840\OneDrive - Honeywell\Desktop\EB Project Reader\EB Reader Project Files\Cytec EB";
            var parser = new EbParser(ebFolderPath);
            parser.ParseEb();

            Console.WriteLine("Hello");
        }
    }
}

