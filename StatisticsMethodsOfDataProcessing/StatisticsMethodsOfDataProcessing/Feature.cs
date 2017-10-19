using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsMethodsOfDataProcessing
{
    public class Feature
    {
        public int Position { get; set; }
        public IList<double> Means { get; set; }
        public IList<double> StandardDeviations { get; set; }
        public double FisherFactor => Math.Abs(Means.First() - Means.ElementAt(1)) / (StandardDeviations.First() + StandardDeviations.ElementAt(1));

        public Feature()
        {
            Means = new List<double>();
            StandardDeviations = new List<double>();
        }
    }
}
