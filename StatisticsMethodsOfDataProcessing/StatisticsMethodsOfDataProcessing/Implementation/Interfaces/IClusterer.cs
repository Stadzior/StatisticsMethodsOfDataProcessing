using StatisticsMethodsOfDataProcessing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMethodsOfDataProcessing.Implementation.Interfaces
{
    public interface IClusterer
    {
        IEnumerable<Cluster> Cluster(FeatureClass featureClass, int k = 1);
    }
}
