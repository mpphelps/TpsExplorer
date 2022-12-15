using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TpsEbReader;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;
using Point = TpsEbReader.Point;


namespace TestWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PointComboBoxViewModel ComboBoxViewModel { get; set; }
        public MainWindow()
        {
            ComboBoxViewModel = new PointComboBoxViewModel();
            InitializeComponent();
        }

        private async void TestButton_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private async void ImportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                ComboBoxViewModel = new PointComboBoxViewModel();
                DialogResult result = dlg.ShowDialog();
                await ComboBoxViewModel.ExecuteEbParserAsync(dlg.SelectedPath);
                var points = ComboBoxViewModel.GetEbPoints();
                var boxes = ComboBoxViewModel.GetEbBoxes();
                UpdateComboBox(points);
                UpdateBoxView(boxes);
                string messageBoxText = $"Successfully Imported {points.Count} Points.";
                string caption = "EB Parser";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Information;
                MessageBoxResult resultMb;
                resultMb = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
            }
        }
        private void UpdateComboBox(List<Point> points)
        {
            PointComboBox.Items.Clear();
            foreach (var point in points)
            {
                PointComboBox.Items.Add(point.Name);
            }
        }
        private void UpdateBoxView(List<Box> boxes)
        {
            BoxComboBox.Items.Clear();
            foreach (var box in boxes)
            {
                BoxComboBox.Items.Add(box.Name);
            }
        }
    }
}
