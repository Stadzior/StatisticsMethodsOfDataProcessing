using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMethodsOfDataProcessing
{
    public static class Extensions
    {
        public static IList<Vector<T>> AsVectors<T>(this Matrix<T> matrix) where T : struct, IEquatable<T>, IFormattable
        {
            var result = new List<Vector<T>>();
            for (int i = 0; i < matrix.RowCount; i++)
                result.Add(matrix.Row(i));
            return result;
        }

        public static IEnumerable<IEnumerable<int>> GetAllRowsPermutations<T>(this Matrix<T> source, int combinationSize) where T : struct, IEquatable<T>, IFormattable
        {
            var rowIndices = new int[source.RowCount];
            for (int i = 0; i < rowIndices.Length; i++)
                rowIndices[i] = i+1;

            return rowIndices.GetAllPermutations(combinationSize);
        }

        public static IEnumerable<IEnumerable<T>> GetAllPermutations<T>(this IEnumerable<T> source, int combinationSize)
        {
            if (combinationSize < 1 || combinationSize > source.Count())
                throw new ArgumentException("Combination size is invalid.");

            List<List<T>> combinations = new List<List<T>>();
            foreach (var item in source)
            {
                if (combinationSize > 1)
                {
                    foreach (var nestedItems in source.Where(x => !x.Equals(item)).GetAllPermutations(combinationSize - 1))
                    {
                        var combination = new List<T> { item };
                        combination.AddRange(nestedItems);
                        if (!combinations.Any(x => x.Count() == combination.Count() && x.All(y => combination.Contains(y))))
                            combinations.Add(combination);
                    }
                }
                else
                {
                    foreach (var item2 in source)
                    {
                        if (!combinations.Select(x => x.First()).Contains(item2))
                            combinations.Add(new List<T> { item2 });
                    }
                }
            }

            return combinations;
        }

        public static Matrix<double> CovarianceMatrix(this Matrix<double> source)
        {
            var meansMatrix = Matrix<double>.Build.Dense(source.RowCount, source.ColumnCount);
            foreach (var feature in source.AsVectors())
            {
                var featureMean = feature.Mean();
                for (int i = 0; i < feature.Count; i++)
                    meansMatrix[source.AsVectors().IndexOf(feature), i] = featureMean;
            }

            var differenceMatrix = source - meansMatrix;
            return differenceMatrix * differenceMatrix.Transpose() / source.ColumnCount;           
        }

        public static double Mean(this Vector<double> source) => source.Sum() / source.Count;
    }
}
