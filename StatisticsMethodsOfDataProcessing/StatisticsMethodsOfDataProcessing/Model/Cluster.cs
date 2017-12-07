using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace StatisticsMethodsOfDataProcessing.Model
{
    public class Cluster : ObservableCollection<Tuple<int, Vector<double>>>
    {
        public Vector<double> Centroid { get; set; }
        public string FeatureClassName { get; set; }
        public Cluster() : base() => CollectionChanged += (sender, e) => RecalculateCentroid();
        public Cluster(Tuple<int, Vector<double>> initialTuple) : this(new List<Tuple<int, Vector<double>>> { initialTuple }) { }
        public Cluster(int index, Vector<double> initialSample) : this(new Tuple<int, Vector<double>>(index, initialSample)) { }
        public Cluster(List<Tuple<int, Vector<double>>> list) : base(list) => CollectionChanged += (sender, e) => RecalculateCentroid();
        public Cluster(IEnumerable<Tuple<int, Vector<double>>> collection) : base(collection) => CollectionChanged += (sender, e) => RecalculateCentroid();

        public IEnumerable<int> SampleIndices => Items.Select(x => x.Item1);
        public IEnumerable<Vector<double>> Samples => Items.Select(x => x.Item2);

        public void Add(int index, Vector<double> sample)
            => Add(new Tuple<int, Vector<double>>(index, sample));

        private void RecalculateCentroid()
        {
            Centroid = Vector<double>.Build.Dense(Samples.First().Count);
            var samples = Samples.ToList();
            for (int i = 0; i < Centroid.Count; i++)
                Centroid[i] = samples.Select(x => x[i]).Mean();
        }
    }
}
