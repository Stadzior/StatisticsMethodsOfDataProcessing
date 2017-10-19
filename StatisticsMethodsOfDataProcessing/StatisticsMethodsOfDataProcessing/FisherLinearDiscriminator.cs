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
        public int[] Discriminate(IEnumerable<Matrix<double>> matrices, int featureCount)
        {
            if (matrices != null && matrices.Any())
            {
                var matrixExpectedDimentions = new Tuple<int, int>(matrices.First().RowCount, matrices.First().ColumnCount);
                if (matrices.Any(x => x.RowCount != matrixExpectedDimentions.Item1 || x.ColumnCount != matrixExpectedDimentions.Item2))
                    throw new InvalidOperationException("Matrices dimentions mismatched.");

                var features = new List<Feature>();
                for (int i = 0; i < matrices.First().RowCount; i++)
                {
                    var feature = new Feature { Position = i };
                    foreach (var matrix in matrices)
                    {
                        var resultTuple = Statistics.MeanStandardDeviation(matrix.Row(i));
                        feature.Means.Add(resultTuple.Item1);
                        feature.StandardDeviations.Add(resultTuple.Item2);
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
