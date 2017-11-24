using MathNet.Numerics.LinearAlgebra;
using StatisticsMethodsOfDataProcessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMethodsOfDataProcessing
{
    public class NearestMeansClassifier : IClassifier
    {
        public string Classify(Vector<double> sample, IEnumerable<FeatureClass> featureClasses, int k = 1)
        {
            return null;
        }
    }
}
