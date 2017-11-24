using MathNet.Numerics.LinearAlgebra;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private FeatureSelectionAlgorithm SelectedAlgorithm = FeatureSelectionAlgorithm.Default;

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
                    var watch = Stopwatch.StartNew();
                    IEnumerable<int> discriminationResults;
                    switch (SelectedAlgorithm)
                    {
                        case FeatureSelectionAlgorithm.SFS:
                            discriminationResults = new FisherLinearDiscriminator().DiscriminateWithSequentialForwardSelection(FeatureClasses, int.Parse(FeaturesSelectionFeaturesCountTextBox.Text));
                            break;
                        case FeatureSelectionAlgorithm.Default:
                        default:
                            discriminationResults = new FisherLinearDiscriminator().Discriminate(FeatureClasses, int.Parse(FeaturesSelectionFeaturesCountTextBox.Text));
                            break;
                    }
                    watch.Stop();
                    if (discriminationResults == null)
                        ResultsTextBox.AppendText("There is no class loaded, use Open file button to load some data.");
                    else
                    {
                        foreach (var featurePosition in discriminationResults)
                            computationResultBuilder.Append($"{featurePosition}, ");
                        computationResultBuilder.Remove(computationResultBuilder.Length - 2, 2);
                        computationResultBuilder.AppendLine($"\t{SelectedAlgorithm.ToString()}\tElapsed time: {watch.Elapsed.Minutes} min {watch.Elapsed.Seconds} s");
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
            var splittedRows = fileContent.Select(x => x.Split(','));
            var classNames = splittedRows.Select(x => x[0]).Distinct();
            var sampleCount = splittedRows
                .Select(x => x.Except(new string[] { x[0] }).Count())
                .Min();

            foreach (var className in classNames)
            {
                var features = splittedRows.Where(x => x[0].Equals(className)).ToList();
                var featureClass = new FeatureClass
                {
                    Name = className,
                    Matrix = Matrix<double>.Build.Dense(features.Count(), sampleCount)
                };

                for (int i = 0; i < features.Count(); i++)
                {
                    var featureData = features[i].Except(new string[] { className }).ToList();
                    for (int j = 0; j < sampleCount; j++)
                    {
                        var sample = featureData[j];
                        featureClass.Matrix[i, j] = double.Parse(sample.Replace('.',','));
                    }
                }
                featureClasses.Add(featureClass);
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
                var splittedRow = row.Split(',');
                for (int i = 0; i < splittedRow.Length; i++)
                {
                    var item = splittedRow[i];
                    matrix[Array.IndexOf(fileContent, row), i] = double.Parse(item);
                }
            }

            return matrix;
        }

        private void FeaturesSelectionRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (FeaturesSelectionFisherRadioButton.IsChecked ?? false)
                SelectedAlgorithm = FeatureSelectionAlgorithm.Default;
            else if (FeaturesSelectionSFSRadioButton.IsChecked ?? false)
                SelectedAlgorithm = FeatureSelectionAlgorithm.SFS;              
        }

        private void SimpleClassificationClassifyButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
