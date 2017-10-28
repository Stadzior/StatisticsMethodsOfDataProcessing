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
  
        public int[] Discriminate(IEnumerable<FeatureClass> featureClasses, int featureCount)
        {
            if (featureClasses != null && featureClasses.Any())
            {
                var matrixExpectedDimensions = new Tuple<int, int>(featureClasses.First().Features.Count, featureClasses.First().SampleCount);
                if (featureClasses.Any(x => x.Features.Count != matrixExpectedDimensions.Item1 || x.SampleCount != matrixExpectedDimensions.Item2))
                    throw new InvalidOperationException("Matrices dimensions mismatched.");

                if (featureCount < 1 || featureCount > featureClasses.First().Features.Count - 1)
                    throw new InvalidOperationException("Feature count invalid.");

                var permutations = featureClasses.First().Matrix.GetAllRowsPermutations(featureCount);
                
            }
            return null;
        }

        public double GetFisherFactor(Matrix<double> source)
        {
            return 0;
        }
    }
}
