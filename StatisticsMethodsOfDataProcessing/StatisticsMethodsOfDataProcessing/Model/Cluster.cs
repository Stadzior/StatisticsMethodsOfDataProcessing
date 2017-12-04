using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace StatisticsMethodsOfDataProcessing.Model
{
    public class Cluster
    {
        public Vector<double> Mean { get; set; }
        public IEnumerable<int> SamplesIndices { get; set; }
    }
}
