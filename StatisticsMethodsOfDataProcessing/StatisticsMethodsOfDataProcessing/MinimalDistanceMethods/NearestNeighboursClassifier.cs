using MathNet.Numerics.LinearAlgebra;
using StatisticsMethodsOfDataProcessing.MinimalDistanceMethods.Interfaces;
using StatisticsMethodsOfDataProcessing.Model;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsMethodsOfDataProcessing.MinimalDistanceMethods
{
    public class NearestNeighboursClassifier : IClassifier
    {
        public string Classify(Vector<double> sourceSample, IEnumerable<FeatureClass> featureClasses, int k = 1)
        {
            if (featureClasses == null || !featureClasses.Any())
                return null;

            var distances = new Dictionary<string, double>();

            foreach (var featureClass in featureClasses)
                foreach (var sample in featureClass.Matrix.ColumnsAsVectors())
                    distances.Add(featureClass.Name, sample.EuclidDistance(sample));

            var shortestDistances = distances.OrderBy(x => x.Value).Take(k).Select(x => x.Key);
            var classesNames = distances
                .Select(x => new { x.Key, Value = distances.Count(y => y.Key == x.Key) })
                .ToDictionary(x => x.Key, x => x.Value);

            return classesNames
                .OrderByDescending(x => x.Value)
                .First().Key;
        }
    }
}
