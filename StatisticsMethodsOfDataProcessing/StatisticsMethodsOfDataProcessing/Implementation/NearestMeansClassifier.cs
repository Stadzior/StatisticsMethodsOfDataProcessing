using MathNet.Numerics.LinearAlgebra;
using StatisticsMethodsOfDataProcessing.MinimalDistanceMethods.Interfaces;
using StatisticsMethodsOfDataProcessing.Model;
using System.Collections.Generic;

namespace StatisticsMethodsOfDataProcessing.MinimalDistanceMethods
{
    public class NearestMeansClassifier : IClassifier
    {
        public string Classify(Vector<double> sample, IEnumerable<FeatureClass> featureClasses, int k = 1)
        {
            return null;
        }
    }
}
