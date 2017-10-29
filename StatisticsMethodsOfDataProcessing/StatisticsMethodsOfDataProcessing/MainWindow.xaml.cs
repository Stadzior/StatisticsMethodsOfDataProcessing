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

        private IList<FeatureClass> FeatureClasses { get; set; }

        #region ("Event handlers")

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
                        FeatureClasses = GetFeatureClasses(fileContent);
                        foreach (var featureClass in FeatureClasses)
                        {
                            if (featureClass.Features.Count < 10 && featureClass.SampleCount < 10)
                                ResultsTextBox.AppendText($"\n{featureClass.ToString()}");
                            else
                                ResultsTextBox.AppendText($"\n{featureClass.Name} class loaded");
                        }
                    }
                }
                catch (Exception)
                {
                    ResultsTextBox.AppendText($"\n File {openFileDialog.FileName} is broken or of not supported format.");
                }
            }
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, ResultsTextBox.Text);
        }

        private void ResultsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResultsTextBox.ScrollToEnd();
        }


        private void FeaturesSelectionComputeButton_Click(object sender, RoutedEventArgs e)
        {
            var validFeatureCountInputted = int.TryParse(FeaturesSelectionFeaturesCountTextBox.Text, out int featureCount);
            if (validFeatureCountInputted)
            {
                var computationResultBuilder = new StringBuilder($"\nBest {featureCount} features: ");
                try
                {
                    var discriminationResults = new FisherLinearDiscriminator().Discriminate(FeatureClasses, int.Parse(FeaturesSelectionFeaturesCountTextBox.Text));
                    if (discriminationResults == null)
                        ResultsTextBox.AppendText("There is no class loaded, use Open file button to load some data.");
                    else
                    {
                        foreach (var featurePosition in discriminationResults)
                            computationResultBuilder.Append($"{featurePosition}, ");
                        computationResultBuilder.Remove(computationResultBuilder.Length - 2, 2);

                        ResultsTextBox.AppendText(computationResultBuilder.ToString());
                    }
                }
                catch (Exception ex)
                {
                    ResultsTextBox.AppendText($"\nError occured while computing: {ex.Message}.");
                }
            }
            else
                ResultsTextBox.AppendText($"Invalid feature count \"{FeaturesSelectionFeaturesCountTextBox.Text}\" inputted.");
        }

        #endregion

        private IList<FeatureClass> GetFeatureClasses(string[] fileContent)
        {
            var featureClasses = new List<FeatureClass>();
            //if (!string.IsNullOrWhiteSpace(fileContent.Last()))
            //    fileContent = fileContent.Union(new string[] { " " }).ToArray();
            List<string> singleMatrixContent = new List<string>();
            foreach (var row in fileContent)
            {
                if (row.Contains("#"))
                {
                    featureClasses.Add(new FeatureClass(GetMatrix(singleMatrixContent.ToArray())));
                    singleMatrixContent.Clear();
                }
                else
                    singleMatrixContent.Add(row);
            }
            return featureClasses;
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
