using StatisticsMethodsOfDataProcessing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;

namespace StatisticsMethodsOfDataProcessing
{
    public class FisherLinearDiscriminator : ILinearDiscriminator
    {
        public int[] Discriminate(IEnumerable<FeatureClass> matrices, int featureCount)
        {
            if (matrices != null && matrices.Any())
            {
                var matrixExpectedDimensions = new Tuple<int, int>(matrices.First().Features.Count, matrices.First().SampleCount);
                if (matrices.Any(x => x.Features.Count != matrixExpectedDimensions.Item1 || x.SampleCount != matrixExpectedDimensions.Item2))
                    throw new InvalidOperationException("Matrices dimensions mismatched.");

                var featureTuples = new List<Tuple<int, int>>();
                for (int i = 0; i < matrices.First().RowCount; i++)
                {
                    var feature = new Feature { Position = i };
                    foreach (var matrix in matrices)
                    {
                        var resultTuple = Statistics.MeanStandardDeviation(matrix.Row(i));
                    }
                    features.Add(feature);
                }
                return features
                    .OrderByDescending(x => x.FisherFactor)
                    .Take(featureCount)
                    .Select(x => x.Position)
                    .ToArray();
            }
            return null;
        }
    }
}
