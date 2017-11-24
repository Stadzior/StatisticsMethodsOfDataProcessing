using MathNet.Numerics.LinearAlgebra;
using StatisticsMethodsOfDataProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMethodsOfDataProcessing.MinimalDistanceMethods.Interfaces
{
    public interface IClassifier
    {
        string Classify(Vector<double> sample, IEnumerable<FeatureClass> featureClasses, int k = 1);
    }
}
