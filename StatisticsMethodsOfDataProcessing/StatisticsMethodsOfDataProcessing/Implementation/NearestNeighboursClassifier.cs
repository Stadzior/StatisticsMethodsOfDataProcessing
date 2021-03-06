﻿using MathNet.Numerics.LinearAlgebra;
using StatisticsMethodsOfDataProcessing.MinimalDistanceMethods.Interfaces;
using StatisticsMethodsOfDataProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StatisticsMethodsOfDataProcessing.MinimalDistanceMethods
{
    public class NearestNeighboursClassifier : IClassifier
    {
        public string Classify(Vector<double> sourceSample, IEnumerable<FeatureClass> featureClasses, int k = 1)
        {
            if (featureClasses == null || !featureClasses.Any())
                return null;

            if (featureClasses.First().Features.Count != sourceSample.Count)
                throw new InvalidOperationException("Sample factors count and class features count mismatched.");

            var distances = new List<KeyValuePair<string, double>>();

            foreach (var featureClass in featureClasses)
                foreach (var sample in featureClass.Samples)
                    distances.Add(new KeyValuePair<string, double>(featureClass.Name, sample.EuclidDistance(sourceSample)));

            var shortestDistances = distances
                .OrderBy(x => x.Value)
                .Take(k);

            var classesNames = shortestDistances
                .Select(x => x.Key)
                .Distinct()
                .Select(x => new { Key = x, Value = shortestDistances.Count(y => y.Key == x) })
                .ToDictionary(x => x.Key, x => x.Value);

            return classesNames
                .OrderByDescending(x => x.Value)
                .First().Key;
        }
    }
}
