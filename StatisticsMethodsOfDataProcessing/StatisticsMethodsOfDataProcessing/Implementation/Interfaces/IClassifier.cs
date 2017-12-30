using MathNet.Numerics.LinearAlgebra;
using StatisticsMethodsOfDataProcessing.MinimalDistanceMethods.Interfaces;
using StatisticsMethodsOfDataProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMethodsOfDataProcessing.Implementation.Interfaces
{
    public interface IClassifier : ISimpleClassifier, IClusterer
    {
        string Classify(Vector<double> sourceSample, IEnumerable<Cluster> clusters);
    }
}
