﻿using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMethodsOfDataProcessing.Interfaces
{
    public interface ILinearDiscriminator
    {
        IEnumerable<int> Discriminate(IList<FeatureClass> matrix, int featureCount);
    }
}