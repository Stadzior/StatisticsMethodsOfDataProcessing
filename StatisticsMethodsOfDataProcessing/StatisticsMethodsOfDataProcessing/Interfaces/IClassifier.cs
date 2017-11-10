using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMethodsOfDataProcessing.Interfaces
{
    public interface IClassifier
    {
        IEnumerable<FeatureClass> Classify(FeatureClass sourceClass);
    }
}
