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

        public static IEnumerable<Tuple<T,T>> GetAllCombinations<T>(IEnumerable<T> source)
        {
            var combinationTuples = new List<Tuple<T, T>>();
            foreach (var item in source)
            {
                foreach (var item2 in source.Where(x => !item.Equals(x)))
                {
                    if (!combinationTuples.Any(x =>
                    (x.Item1.Equals(item) && x.Item2.Equals(item2)) ||
                    (x.Item1.Equals(item2) && x.Item2.Equals(item))))
                        combinationTuples.Add(new Tuple<T, T>(item, item2));
                }
            }
            return combinationTuples;
        }
    }
}
