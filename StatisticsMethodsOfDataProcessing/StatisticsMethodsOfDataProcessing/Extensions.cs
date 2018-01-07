using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using StatisticsMethodsOfDataProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMethodsOfDataProcessing
{
    public static class Extensions
    {
        public static IList<Vector<T>> RowsAsVectors<T>(this Matrix<T> matrix) where T : struct, IEquatable<T>, IFormattable
        {
            var result = new List<Vector<T>>();
            for (int i = 0; i < matrix.RowCount; i++)
                result.Add(matrix.Row(i));
            return result;
        }

        public static IList<Vector<T>> ColumnsAsVectors<T>(this Matrix<T> matrix) where T : struct, IEquatable<T>, IFormattable
        {
            var result = new List<Vector<T>>();
            for (int i = 0; i < matrix.ColumnCount; i++)
                result.Add(matrix.Column(i));
            return result;
        }

        public static IEnumerable<int> GetRowsIndices<T>(this Matrix<T> source) where T : struct, IEquatable<T>, IFormattable
            => Enumerable.Range(0, source.RowCount);

        public static IEnumerable<int> GetColumnsIndices<T>(this Matrix<T> source) where T : struct, IEquatable<T>, IFormattable
            => Enumerable.Range(0, source.ColumnCount);

        public static IEnumerable<IList<T>> GetAllCombinations<T>(this IEnumerable<T> source, int combinationSize)
        {
            if (combinationSize < 1 || combinationSize > source.Count())
                throw new ArgumentException("Combination size is invalid.");

            List<List<T>> combinations = new List<List<T>>();
            foreach (var item in source)
            {
                if (combinationSize > 1)
                {
                    var nestedCombinations = source.Where(x => !x.Equals(item)).GetAllCombinations(combinationSize - 1).ToList();
                    foreach (var nestedItems in nestedCombinations)
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
            foreach (var feature in source.RowsAsVectors())
            {
                var featureMean = feature.Mean();
                for (int i = 0; i < feature.Count; i++)
                    meansMatrix[source.RowsAsVectors().IndexOf(feature), i] = featureMean;
            }

            var differenceMatrix = source - meansMatrix;
            return differenceMatrix * differenceMatrix.Transpose() / source.ColumnCount;
        }

        public static Matrix<double> MeansMatrix(this IList<FeatureClass> source)
        {
            if (source != null && source.Any())
            {
                var matrixExpectedDimensions = new Tuple<int, int>(source.First().Features.Count, source.First().Samples.Count);
                if (source.Any(x => x.Features.Count != matrixExpectedDimensions.Item1 || x.Samples.Count != matrixExpectedDimensions.Item2))
                    throw new InvalidOperationException("Matrices dimensions mismatched.");

                var meansMatrix = Matrix<double>.Build.Dense(source.First().Features.Count, source.Count());
                for (int i = 0; i < source.First().Features.Count; i++)
                {
                    for (int j = 0; j < source.Count(); j++)
                    {
                        meansMatrix[i, j] = source[j].Features[i].Mean();
                    }
                }
                return meansMatrix;
            }
            else
                return null;
        }

        public static Matrix<T> SubMatrix<T>(this Matrix<T> source, IList<int> rowsIndices = null, IList<int> columnsIndices = null) where T : struct, IEquatable<T>, IFormattable
        {
            if (rowsIndices == null)
            {
                rowsIndices = new List<int>();
                for (int i = 0; i < source.RowCount; i++)
                    rowsIndices.Add(i);
            }

            if (columnsIndices == null)
            {
                columnsIndices = new List<int>();
                for (int i = 0; i < source.ColumnCount; i++)
                    columnsIndices.Add(i);
            }

            var subMatrix = Matrix<T>.Build.Dense(rowsIndices.Count(), columnsIndices.Count());
            for (int i = 0; i < rowsIndices.Count(); i++)
                for (int j = 0; j < columnsIndices.Count(); j++)
                    subMatrix[i, j] = source[rowsIndices.ElementAt(i), columnsIndices.ElementAt(j)];

            return subMatrix;
        }

        public static double EuclidDistance(this Vector<double> source, Vector<double> target)
        {
            if (source.Count != target.Count)
                throw new InvalidOperationException("Vectors coordinate count mismatched.");

            var result = 0.0;
            for (int i = 0; i < source.Count; i++)
                result += Math.Pow(target[i] - source[i], 2);
            return Math.Sqrt(result);
        }

        public static double MahalanobisDistance(this Vector<double> source, Matrix<double> target, Vector<double> centroid)
        {
            if (source.Count() != target.RowCount)
                throw new InvalidOperationException("Vector coordinate count and matrix rows count mismatched.");

            var sourceMatrix = Matrix<double>.Build.Dense(target.RowCount, 1, (x,y) => source[x]);
            var meansOfEuclidDistances = Matrix<double>.Build.Dense(target.RowCount, 1, (x, y) => source.EuclidDistance(centroid));
            return ((sourceMatrix - meansOfEuclidDistances).Transpose() * target.CovarianceMatrix().Inverse() * (sourceMatrix - meansOfEuclidDistances))[0,0];
        }

        public static IEnumerable<T> ExceptWithDuplicates<T>(this IEnumerable<T> source, IEnumerable<T> exceptionalCollection)
            => source.Where(x => !exceptionalCollection.Any(y => y.Equals(x)));

        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> source, int count)
        {
            var randoms = new List<T>(count);
            var randomsIndices = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                var range = Enumerable.Range(0, source.Count() - 1).Where(x => !randomsIndices.Contains(x));
                var randomIndex = new Random().Next(0, source.Count() - 1 - randomsIndices.Count());
                randomsIndices.Add(randomIndex);
                randoms.Add(source.ElementAt(randomIndex));
            }
            return randoms;
        }

        public static T TakeRandom<T>(this IEnumerable<T> source)
            => source.TakeRandom(1).First();

        public static string ToReadableString<T>(this Vector<T> source) where T:struct, IEquatable<T>, IFormattable
        {
            var readableStringBuilder = new StringBuilder("[");
            foreach (var factor in source)
                readableStringBuilder
                    .Append(factor)
                    .Append(",");
            readableStringBuilder.Remove(readableStringBuilder.Length - 1, 1);
            readableStringBuilder.Append("]");
            return readableStringBuilder.ToString();
        }
    }
}
