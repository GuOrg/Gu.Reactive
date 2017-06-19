namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    public class MinTrackerSimple
    {
        private ObservableCollection<int> ints1 = new ObservableBatchCollection<int>();
        private ObservableCollection<int> ints2 = new ObservableBatchCollection<int>();

        [GlobalSetup]
        public void SetupData()
        {
            foreach (var ints in new[] { this.ints1, this.ints2 })
            {
                ints.Clear();
                for (int i = 0; i < 10; i++)
                {
                    ints.Add(i);
                }
            }
        }

        [Benchmark(Baseline = true)]
        public int? Linq()
        {
            var min = this.ints1.Min(x => x);
            this.ints1.Add(5);
            return Math.Min(min, this.ints1.Min(x => x));
        }

        [Benchmark]
        public int? Tracker()
        {
            using (var tracker = this.ints2.TrackMin())
            {
                this.ints2.Add(5);
                return tracker.Value;
            }
        }
    }
}