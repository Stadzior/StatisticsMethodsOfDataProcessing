using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using StatisticsMethodsOfDataProcessing.Model;
using StatisticsMethodsOfDataProcessing.MinimalDistanceMethods.Interfaces;
using System.Linq;
using System;

namespace StatisticsMethodsOfDataProcessing.MinimalDistanceMethods
{
    public class NearestMeansWithDispertionClassifier : NearestMeansClassifier
    {
        public override string Classify(Vector<double> sourceSample, IEnumerable<Cluster> clusters)
        {
            return clusters
                .OrderBy(x => sourceSample.MahalanobisDistance(x.ToMatrix(), x.Centroid))
                .First()
                .FeatureClassName;
        }
    }
}
