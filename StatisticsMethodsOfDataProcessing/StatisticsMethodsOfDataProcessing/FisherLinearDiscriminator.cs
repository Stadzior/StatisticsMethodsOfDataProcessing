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
  
        public IEnumerable<Tuple<IEnumerable<int>, double>> Discriminate(IList<FeatureClass> featureClasses, int featureCount)
        {
            if (featureClasses != null && featureClasses.Any())
            {
                var matrixExpectedDimensions = new Tuple<int, int>(featureClasses.First().Features.Count, featureClasses.First().SampleCount);
                if (featureClasses.Any(x => x.Features.Count != matrixExpectedDimensions.Item1 || x.SampleCount != matrixExpectedDimensions.Item2))
                    throw new InvalidOperationException("Matrices dimensions mismatched.");

                if (featureCount < 1 || featureCount > featureClasses.First().Features.Count - 1)
                    throw new InvalidOperationException("Feature count invalid.");

                var fisherFactorTuples = new List<Tuple<IEnumerable<int>, double>>();
                foreach (var permutation in featureClasses.First().Matrix.GetAllRowsPermutations(featureCount))
                    fisherFactorTuples.Add(new Tuple<IEnumerable<int>, double>(permutation, GetFisherFactor(featureClasses, permutation)));

                return fisherFactorTuples;
            }
            else
                return null;
        }

        private static double GetFisherFactor(IList<FeatureClass> featureClasses, IList<int> permutation)
        {
            var numerator = featureClasses.MeansMatrix().CovarianceMatrix().Determinant();
            var denominator = featureClasses
                .Select(x => x.Matrix.SubMatrix(permutation)
                .CovarianceMatrix()
                .Determinant())
                .Sum();
            var fisherFactor = numerator / denominator;
            return fisherFactor;
        }
    }
}
