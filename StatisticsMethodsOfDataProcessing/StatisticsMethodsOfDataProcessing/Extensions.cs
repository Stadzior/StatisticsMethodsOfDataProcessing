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
    }
}
