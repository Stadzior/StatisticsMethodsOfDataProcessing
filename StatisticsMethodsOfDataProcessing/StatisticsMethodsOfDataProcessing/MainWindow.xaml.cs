using MathNet.Numerics.LinearAlgebra;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace StatisticsMethodsOfDataProcessing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Matrix<double> Matrix { get; set; }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string[] fileContent = null;
                try
                {
                    fileContent = File.ReadAllLines(openFileDialog.FileName);
                } catch (Exception)
                {
                    ResultsTextBox.Text += $"\n File {openFileDialog.FileName} is broken or of not supported format.";
                }
                
                if (fileContent != null && fileContent.Any())
                {
                    var matrixHeight = fileContent.Length;
                    var matrixWidth = fileContent.First().Replace(" ", "").Length;
                    //Matrix = Matrix<double>.Build.
                }
            }
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, ResultsTextBox.Text);
        }
    }
}
