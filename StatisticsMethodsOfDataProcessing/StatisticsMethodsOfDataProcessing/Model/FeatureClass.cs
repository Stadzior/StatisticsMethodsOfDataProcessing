using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMethodsOfDataProcessing.Model
{
    public class FeatureClass
    {
        public string Name { get; set; }
        public Matrix<double> Matrix { get; set; }
        public IList<Vector<double>> Features => Matrix.RowsAsVectors();
        public IList<Vector<double>> Samples => Matrix.ColumnsAsVectors();

        public override string ToString()
            => new StringBuilder(Name)
                .AppendLine()
                .Append(Matrix.ToString())
                .ToString();
    }
}
