using MathNet.Numerics.LinearAlgebra;

namespace StatisticsMethodsOfDataProcessing.Model
{
    public class ClassificationResult
    {
        public string ClassifiedClassName { get; set; }
        public string ActualClassName { get; set; }
        public Vector<double> Sample { get; set; }
    }
}
