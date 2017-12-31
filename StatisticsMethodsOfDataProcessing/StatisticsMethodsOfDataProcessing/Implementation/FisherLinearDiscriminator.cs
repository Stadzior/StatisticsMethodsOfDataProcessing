using System;
using System.Collections.Generic;
using System.Linq;
using StatisticsMethodsOfDataProcessing.Model;
using StatisticsMethodsOfDataProcessing.MinimalDistanceMethods.Interfaces;

namespace StatisticsMethodsOfDataProcessing.MinimalDistanceMethods
{
    public class FisherLinearDiscriminator : ILinearDiscriminator
    { 
        public IEnumerable<int> Discriminate(IList<FeatureClass> featureClasses, int featureCount)
        {
            if (featureClasses != null && featureClasses.Any())
            {
                var matrixExpectedDimensions = new Tuple<int, int>(featureClasses.First().Features.Count, featureClasses.First().Samples.Count);
                if (featureClasses.Any(x => x.Features.Count != matrixExpectedDimensions.Item1 || x.Samples.Count != matrixExpectedDimensions.Item2))
                    throw new InvalidOperationException("Matrices dimensions mismatched.");

                if (featureCount < 1 || featureCount > featureClasses.First().Features.Count)
                    throw new InvalidOperationException("Feature count invalid.");

                if (featureCount == featureClasses.First().Features.Count)
                    return featureClasses.First().Matrix.GetRowsIndices();
                else
                {
                    var fisherFactorTuples = new List<Tuple<IEnumerable<int>, double>>();
                    var combinations = featureClasses.First().Matrix.GetRowsIndices().GetAllCombinations(featureCount).ToList();
                    foreach (var combination in combinations)
                        fisherFactorTuples.Add(new Tuple<IEnumerable<int>, double>(combination, GetFisherFactor(featureClasses, combination)));

                    return fisherFactorTuples
                        .OrderByDescending(x => x.Item2)
                        .First()
                        .Item1;
                }
            }
            else
                return null;
        }

        private double GetFisherFactor(IList<FeatureClass> featureClasses, IList<int> combination)
        {
            var numerator = featureClasses
                .MeansMatrix()
                .SubMatrix(combination)
                .CovarianceMatrix()
                .Determinant();

            var denominator = featureClasses
                .Select(x => x.Matrix.SubMatrix(combination)
                .CovarianceMatrix()
                .Determinant())
                .Sum();

            var fisherFactor = numerator / denominator;
            return fisherFactor;
        }

        public IEnumerable<int> DiscriminateWithSequentialForwardSelection(IList<FeatureClass> featureClasses, int featureCount)
        {
            if (featureCount < 1)
                throw new ArgumentOutOfRangeException("Feature count can't be less than 1.");
            else if (featureCount < 2)
                return new List<int>(Discriminate(featureClasses, featureCount));
            else
            {
                var bestNFeatures = DiscriminateWithSequentialForwardSelection(featureClasses, featureCount - 1);
                var combinations = featureClasses
                    .First()
                    .Matrix
                    .GetRowsIndices()
                    .GetAllCombinations(featureCount)
                    .Where(x => bestNFeatures.All(y => x.Contains(y)));

                var fisherFactorTuples = new List<Tuple<IEnumerable<int>, double>>();
                foreach (var combination in combinations)
                    fisherFactorTuples.Add(new Tuple<IEnumerable<int>, double>(combination, GetFisherFactor(featureClasses, combination)));

                return fisherFactorTuples
                    .OrderByDescending(x => x.Item2)
                    .First()
                    .Item1;
            }
        }
    }
}
