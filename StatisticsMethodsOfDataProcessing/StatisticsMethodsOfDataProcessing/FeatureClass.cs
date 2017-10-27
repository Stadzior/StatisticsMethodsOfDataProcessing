using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMethodsOfDataProcessing
{
    public class FeatureClass
    {
        public string Name { get; set; }
        public Matrix<double> Matrix { get; set; }
        public IList<Vector<double>> Features => Matrix.AsVectors();
        public int SampleCount => Matrix.ColumnCount;

        public FeatureClass(Matrix<double> matrix)
        {
            Matrix = matrix;
        }
    }
}
