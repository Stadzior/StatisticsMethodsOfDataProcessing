using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using StatisticsMethodsOfDataProcessing.Model;
using StatisticsMethodsOfDataProcessing.MinimalDistanceMethods.Interfaces;
using System.Linq;
using System;

namespace StatisticsMethodsOfDataProcessing.MinimalDistanceMethods
{
    public class NearestMeansWithDispertionClassifier : NearestMeansClassifier, IClassifier
    {
        public override string Classify(Vector<double> sourceSample, IEnumerable<FeatureClass> featureClasses, int k = 1)
        {
            if (featureClasses == null || !featureClasses.Any())
                return null;

            if (featureClasses.First().Features.Count != sourceSample.Count)
                throw new InvalidOperationException("Sample factors count and class features count mismatched.");

            var clusters = featureClasses
                .SelectMany(x => Cluster(x, k));

            return clusters
                .OrderBy(x => sourceSample.MahalanobisDistance(x.ToMatrix(), x.Centroid))
                .First()
                .FeatureClassName;
        }
    }
}
