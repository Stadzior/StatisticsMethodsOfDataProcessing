using MathNet.Numerics.LinearAlgebra;
using StatisticsMethodsOfDataProcessing.Implementation.Interfaces;
using StatisticsMethodsOfDataProcessing.MinimalDistanceMethods.Interfaces;
using StatisticsMethodsOfDataProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsMethodsOfDataProcessing.MinimalDistanceMethods
{
    public class NearestMeansClassifier : IClassifier, IClusterer
    {
        public string Classify(Vector<double> sourceSample, IEnumerable<FeatureClass> featureClasses, int k = 1)
        {
            if (featureClasses == null || !featureClasses.Any())
                return null;

            if (featureClasses.First().Features.Count != sourceSample.Count)
                throw new InvalidOperationException("Sample factors count and class features count mismatched.");

            var clusters = featureClasses.Select(x => Cluster(x, k));
        }

        public IEnumerable<Cluster> Cluster(FeatureClass featureClass, int k = 1)
        {
            var randomIndices = Enumerable.Range(0, featureClass.Samples.Count - 1)
                .TakeRandom(k)
                .ToList();

            var clusters = new List<Cluster>();

            for (int i = 0; i < k - 1; i++)
                clusters.Add(new Cluster
                {
                    Mean = featureClass.Samples[randomIndices[i]],
                    SamplesIndices = new List<int> { randomIndices[i] }
                });



            foreach (var sample in featureClass.Samples)
            {

            }
        }
    }
}
