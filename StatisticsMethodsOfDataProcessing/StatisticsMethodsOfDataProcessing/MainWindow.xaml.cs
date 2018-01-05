using MathNet.Numerics.LinearAlgebra;
using Microsoft.Win32;
using StatisticsMethodsOfDataProcessing.Enums;
using StatisticsMethodsOfDataProcessing.Implementation.Interfaces;
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
                            if (featureClass.Features.Count < 10 && featureClass.Samples.Count < 10)
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

        private void ClassificationClassifyButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Value = 0;

            if (FeatureClasses == null || !FeatureClasses.Any())
            {
                ResultsTextBox.AppendText($"{Environment.NewLine}There is no class loaded, use open file button to load some data.");
                return;
            }

            int k = 1;
            try
            {
                k = int.Parse(ClassificationKTextBox.Text);
            }
            catch (Exception)
            {
                ResultsTextBox.AppendText($"{Environment.NewLine}Incorrect format, k should be an integer.");
                return;
            }

            if ((ClassificationAlgorithm)ClassificationClassifierComboBox.SelectedIndex == ClassificationAlgorithm.NearestNeighbours)
            {
                Vector<double> sample = null;
                try
                {
                    sample = Vector<double>.Build.DenseOfEnumerable(ClassificationSampleTextBox.Text.Split(',').Select(x => double.Parse(x.Replace('.', ','))));
                }
                catch (Exception)
                {
                    ResultsTextBox.AppendText($"{Environment.NewLine}Incorrect format of sample, should be:{Environment.NewLine}- \"double,double,double...\" e.g. \"1.023,34.232,43.123\"");
                    return;
                }

                var classifier = new NearestNeighboursClassifier();
                var classificationResultClassName = string.Empty;
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
            else
            {
                if (!int.TryParse(ClassificationTrainingPartTextBox.Text, out int trainingPart) || trainingPart < 0 || trainingPart > 100)
                    throw new ArgumentOutOfRangeException("Training part should be in range 0-100.");
                IClassifier classifier = null;
                switch ((ClassificationAlgorithm)ClassificationClassifierComboBox.SelectedIndex)
                {
                    case ClassificationAlgorithm.NearestMeans:
                        {
                            classifier = new NearestMeansClassifier();
                            break;
                        }
                    case ClassificationAlgorithm.NearestMeansWithDispertion:
                        {
                            classifier = new NearestMeansWithDispertionClassifier();
                            break;
                        }
                }

                var trainingPartCount = (int) Math.Ceiling(FeatureClasses.First().Samples.Count * (trainingPart / 100.0));
                var trainingParts = FeatureClasses
                    .Select(x => new FeatureClass
                    {
                        Name = x.Name,
                        Matrix = x.Matrix.SubMatrix(0, x.Features.Count, 0, trainingPartCount)
                    });

                var samplesToClassify = FeatureClasses
                    .SelectMany(x => x.Samples.Skip(trainingPartCount)
                    .Select(y => new KeyValuePair<string, Vector<double>>(x.Name, y)));

                ProgressBar.Minimum = 0;
                ProgressBar.Maximum = samplesToClassify.Count();

                try
                {
                    var clusters = trainingParts
                        .SelectMany(x => classifier.Cluster(x, k))
                        .ToList(); //Preventing randomisation in clustering to have an impact on further code.

                    var classificationResults = samplesToClassify
                        .Select(x =>
                        {
                            Dispatcher.BeginInvoke(new Action(() => ProgressBar.Value += 1));
                            return new
                            {
                                ClassifiedClassName = classifier.Classify(x.Value, clusters),
                                ActualClassName = x.Key,
                                Sample = x.Value
                            };
                        })
                        .ToList();

                    if (classificationResults.Count() < 20)
                    {
                        foreach (var result in classificationResults)
                        {
                            if (result.ActualClassName.Equals(result.ClassifiedClassName))
                                ResultsTextBox.AppendText($"{Environment.NewLine}Sample [{result.Sample.ToReadableString()}] has been correctly classified to class {result.ClassifiedClassName}.");
                            else
                                ResultsTextBox.AppendText($"{Environment.NewLine}Sample {result.Sample.ToReadableString()} has been classified to class {result.ClassifiedClassName} but actually belongs to class {result.ActualClassName}.");
                        }
                    }
                    else
                    {
                        var correctlyClassifiedSamplesCount = classificationResults.Where(x => x.ActualClassName.Equals(x.ClassifiedClassName)).Count();
                        ResultsTextBox.AppendText($"{Environment.NewLine} {correctlyClassifiedSamplesCount} out of {classificationResults.Count()} ({((correctlyClassifiedSamplesCount / (double) classificationResults.Count()) * 100).ToString("###.##")}%) samples was classified correctly.");
                    }
                }
                catch (Exception ex)
                {
                    ResultsTextBox.AppendText($"{Environment.NewLine}Error occured while classifying: {ex.Message}");
                    return;
                }
            }
        }

        private void ClassificationClassifierComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClassificationTrainingPartTextBox != null)
                ClassificationTrainingPartTextBox.IsEnabled = (sender as ComboBox)?.SelectedIndex != (int)ClassificationAlgorithm.NearestNeighbours
                    && (ClassificationCustomRadioButton.IsChecked ?? false);
            if (ClassificationSampleTextBox != null)
                ClassificationSampleTextBox.IsEnabled = (sender as ComboBox)?.SelectedIndex == (int)ClassificationAlgorithm.NearestNeighbours;
        }

        private void ClassificationRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ClassificationTrainingPartTextBox != null)
                ClassificationTrainingPartTextBox.IsEnabled = ClassificationClassifierComboBox.SelectedIndex != (int)ClassificationAlgorithm.NearestNeighbours
                    && (ClassificationCustomRadioButton.IsChecked ?? false);
        }

        #endregion

        private IList<FeatureClass> GetFeatureClasses(string[] fileContent)
        {
            var featureClasses = new List<FeatureClass>();
            var splittedRows = fileContent.Select(x => x.Split(','));
            var classNames = splittedRows.Select(x => x[0]).Distinct();
            var featureCount = splittedRows
                .Select(x => x.ExceptWithDuplicates(new string[] { x[0] }).Count())
                .Min();

            foreach (var className in classNames)
            {
                var samples = splittedRows.Where(x => x[0].Equals(className)).ToList();
                var featureClass = new FeatureClass
                {
                    Name = className,
                    Matrix = Matrix<double>.Build.Dense(featureCount, samples.Count())
                };

                for (int i = 0; i < samples.Count(); i++)
                {
                    var sampleData = samples[i].ExceptWithDuplicates(new string[] { className }).ToList();
                    for (int j = 0; j < featureCount; j++)
                    {
                        var factor = sampleData[j];
                        featureClass.Matrix[j,i] = double.Parse(factor.Replace('.',','));
                    }
                }
                featureClasses.Add(featureClass);
            }
            return featureClasses;
        }
    }
}
