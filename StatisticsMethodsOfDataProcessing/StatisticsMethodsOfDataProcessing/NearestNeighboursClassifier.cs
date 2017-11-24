using MathNet.Numerics.LinearAlgebra;
using StatisticsMethodsOfDataProcessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMethodsOfDataProcessing
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

            var classesNames = distances.OrderBy(x => x.Value).Take(k).Select(x => x.Key);

            return null;

        }
    }
}
