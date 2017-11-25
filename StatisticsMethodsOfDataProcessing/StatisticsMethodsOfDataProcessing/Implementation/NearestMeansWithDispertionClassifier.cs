using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using StatisticsMethodsOfDataProcessing.Model;
using StatisticsMethodsOfDataProcessing.MinimalDistanceMethods.Interfaces;

namespace StatisticsMethodsOfDataProcessing.MinimalDistanceMethods
{
    public class NearestMeansWithDispertionClassifier : IClassifier
    {
        public string Classify(Vector<double> sample, IEnumerable<FeatureClass> featureClasses, int k = 1)
        {
            return null;
        }
    }
}
