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
        public Func<object, object, double> DistanceCalculationFunction { get; set; }

        public string Classify(Vector<double> sourceSample, IEnumerable<FeatureClass> featureClasses, int k = 1)
        {
            if (featureClasses == null || !featureClasses.Any())
                return null;
            
            if (featureClasses.First().Features.Count != sourceSample.Count)
                throw new InvalidOperationException("Sample factors count and class features count mismatched.");

            var clusters = featureClasses
                .SelectMany(x => Cluster(x, k));

            return clusters
                .OrderByDescending(x => x.Centroid.EuclidDistance(sourceSample))
                .First()
                .FeatureClassName;
        }
        
        public IEnumerable<Cluster> Cluster(FeatureClass featureClass, int k = 1)
        {
            var samples = featureClass.Samples;

            var randomIndices = Enumerable.Range(0, samples.Count - 1)
                .TakeRandom(k)
                .ToList();

            var clusters = new List<Cluster>();

            for (int i = 0; i < k; i++)
            {
                var cluster = new Cluster(randomIndices[i], featureClass.Samples[randomIndices[i]])
                {
                    FeatureClassName = featureClass.Name,
                    Centroid = featureClass.Samples[randomIndices[i]]
                };
                clusters.Add(cluster);
                samples.RemoveAt(randomIndices[i]);
            }

            foreach (var sample in samples)
            {
                var chosenCluster = clusters
                    .First(x => x.Centroid.EuclidDistance(sample) == clusters.Min(y => y.Centroid.EuclidDistance(sample)));
                chosenCluster.Add(featureClass.Samples.IndexOf(sample), sample);
            }

            return clusters;
        }
    }
}
