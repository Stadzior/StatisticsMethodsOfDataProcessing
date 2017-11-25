using MathNet.Numerics.LinearAlgebra;
using Microsoft.Win32;
using StatisticsMethodsOfDataProcessing.Enums;
using StatisticsMethodsOfDataProcessing.MinimalDistanceMethods;
using StatisticsMethodsOfDataProcessing.MinimalDistanceMethods.Interfaces;
using StatisticsMethodsOfDataProcessing.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

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
            var openFileDialog = new OpenFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 0
            };

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
                                ResultsTextBox.AppendText($"{Environment.NewLine}{featureClass.ToString()}");
                            else
                                ResultsTextBox.AppendText($"{Environment.NewLine}{featureClass.Name} class loaded");
                        }
                    }
                }
                catch (Exception)
                {
                    ResultsTextBox.AppendText($"{Environment.NewLine} File {openFileDialog.FileName} is broken or of not supported format.");
                }
            }
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 0
            };

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, ResultsTextBox.Text);
        }

        private void ResultsTextBox_TextChanged(object sender, TextChangedEventArgs e)
            => ResultsTextBox.ScrollToEnd();

        private void FeaturesSelectionComputeButton_Click(object sender, RoutedEventArgs e)
        {
            if (FeatureClasses == null || !FeatureClasses.Any())
            {
                ResultsTextBox.AppendText($"{Environment.NewLine}There is no class loaded, use Open file button to load some data.");
                return;
            }

            var validFeatureCountInputted = int.TryParse(FeaturesSelectionFeaturesCountTextBox.Text, out int featureCount);
            if (validFeatureCountInputted)
            {
                var computationResultBuilder = new StringBuilder($"{Environment.NewLine}Best {featureCount} features: ");
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

                    foreach (var featurePosition in discriminationResults)
                        computationResultBuilder.Append($"{featurePosition}, ");
                    computationResultBuilder.Remove(computationResultBuilder.Length - 2, 2);
                    computationResultBuilder.AppendLine($"\t{SelectedAlgorithm.ToString()}\tElapsed time: {watch.Elapsed.Minutes} min {watch.Elapsed.Seconds} s");
                    ResultsTextBox.AppendText(computationResultBuilder.ToString());
                }
                catch (Exception ex)
                {
                    ResultsTextBox.AppendText($"{Environment.NewLine}Error occured while computing: {ex.Message}.");
                }
            }
            else
                ResultsTextBox.AppendText($"{Environment.NewLine}Invalid feature count \"{FeaturesSelectionFeaturesCountTextBox.Text}\" inputted.");
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
            if (FeatureClasses == null || !FeatureClasses.Any())
            {
                ResultsTextBox.AppendText($"{Environment.NewLine}There is no class loaded, use Open file button to load some data.");
                return;
            }

            Vector<double> sample = null;
            int k = 1;
            try
            {
                sample = Vector<double>.Build.DenseOfEnumerable(SimpleClassificationSampleTextBox.Text.Split(',').Select(x => double.Parse(x)));
                k = int.Parse(SimpleClassificationKTextBox.Text);
            }
            catch (Exception)
            {
                ResultsTextBox.AppendText($"{Environment.NewLine}Incorrect format of sample or k use:{Environment.NewLine}- \"double,double,double...\" e.g. \"1.023,34.232,43.123\" for sample{Environment.NewLine}- integer for k");
                return;
            }

            IClassifier classifier = null;
            switch ((ClassifyingMethod)SimpleClassificationClassifierComboBox.SelectedIndex)
            {
                case ClassifyingMethod.NearestNeighbours:
                    classifier = new NearestNeighboursClassifier();
                    break;
                case ClassifyingMethod.NearestMeans:
                    classifier = new NearestMeansClassifier();
                    break;
                case ClassifyingMethod.NearestMeansWithDispertion:
                    classifier = new NearestMeansWithDispertionClassifier();
                    break;
            }
            string classificationResultClassName = string.Empty;
            try
            {
                classificationResultClassName = classifier.Classify(sample, FeatureClasses, k);
            }
            catch (Exception ex)
            {
                ResultsTextBox.AppendText($"{Environment.NewLine}Error occured while classifying: {ex.Message}");
                return;
            }

            ResultsTextBox.AppendText($"{Environment.NewLine}Sample has been classified to class: {classificationResultClassName}");
        }

        #endregion

        private IList<FeatureClass> GetFeatureClasses(string[] fileContent)
        {
            var featureClasses = new List<FeatureClass>();
            var splittedRows = fileContent.Select(x => x.Split(','));
            var classNames = splittedRows.Select(x => x[0]).Distinct();
            var sampleCount = splittedRows
                .Select(x => x.ExceptWithDuplicates(new string[] { x[0] }).Count())
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
                    var featureData = features[i].ExceptWithDuplicates(new string[] { className }).ToList();
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
    }
}
