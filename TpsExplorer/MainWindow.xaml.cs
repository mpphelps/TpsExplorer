using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TpsEbReader;

namespace TpsExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var ebFolderPath = @"C:\Users\h111840\OneDrive - Honeywell\Desktop\EB Project Reader\EB Reader Project Files\Cytec EB";
            var parser = new EbParser(ebFolderPath);
            parser.ParseEb();
            Console.WriteLine("Hello");
        }
    }
}
