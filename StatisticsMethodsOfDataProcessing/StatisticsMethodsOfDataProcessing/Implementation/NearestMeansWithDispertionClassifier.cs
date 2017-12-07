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
        public override IEnumerable<Cluster> Cluster(FeatureClass featureClass, int k = 1)
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
                    .First(x => x.Centroid.EuclidDistance(sample) == clusters.Min(y => y.Centroid.MahalonobisDistance()));
                chosenCluster.Add(featureClass.Samples.IndexOf(sample), sample);
            }

            return clusters;
        }
    }
}
