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

        private IList<Matrix<double>> Matrices { get; set; }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string[] fileContent = null;
                try
                {
                    fileContent = File.ReadAllLines(openFileDialog.FileName);            
                    if (fileContent != null && fileContent.Any())
                    {
                        Matrices = GetMatrices(fileContent);
                        foreach (var matrix in Matrices)
                        {
                            ResultsTextBox.Text += $"\n{matrix.ToString()}";
                        }
                    }
                }
                catch (Exception)
                {
                    ResultsTextBox.Text += $"\n File {openFileDialog.FileName} is broken or of not supported format.";
                }
            }
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, ResultsTextBox.Text);
        }

        private IList<Matrix<double>> GetMatrices(string[] fileContent)
        {
            var matrices = new List<Matrix<double>>();
            if (!string.IsNullOrWhiteSpace(fileContent.Last()))
                fileContent = fileContent.Union(new string[] { " " }).ToArray();
            List<string> singleMatrixContent = new List<string>();
            foreach (var row in fileContent)
            {
                if (string.IsNullOrWhiteSpace(row))
                {
                    matrices.Add(GetMatrix(singleMatrixContent.ToArray()));
                    singleMatrixContent.Clear();
                }
                else
                    singleMatrixContent.Add(row);
            }
            return matrices;
        }

        private static Matrix<double> GetMatrix(string[] fileContent)
        {
            var matrixHeight = fileContent.Length;
            var matrixWidth = fileContent
                .First()
                .Trim()
                .Count(x => string.IsNullOrWhiteSpace(x.ToString())) + 1;

            var matrix = Matrix<double>.Build.Dense(matrixHeight, matrixWidth);
            foreach (var row in fileContent)
            {
                var splittedRow = row.Split(' ');
                for (int i = 0; i < splittedRow.Length; i++)
                {
                    var item = splittedRow[i];
                    matrix[Array.IndexOf(fileContent, row), i] = double.Parse(item);
                }
            }

            return matrix;
        }
    }
}
