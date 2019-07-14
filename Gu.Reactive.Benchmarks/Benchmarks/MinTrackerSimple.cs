namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    public class MinTrackerSimple
    {
        private readonly ObservableCollection<int> ints1 = new ObservableBatchCollection<int>();
        private readonly ObservableCollection<int> ints2 = new ObservableBatchCollection<int>();

        [GlobalSetup]
        public void SetupData()
        {
            foreach (var ints in new[] { this.ints1, this.ints2 })
            {
                ints.Clear();
                for (int i = 0; i < 1000; i++)
                {
                    ints.Add(i);
                }
            }
        }

        [Benchmark(Baseline = true)]
        public int? Linq()
        {
            var min = this.ints1.Min(x => x);
            Update(this.ints1);
            return Math.Min(min, this.ints1.Min(x => x));
        }

        [Benchmark]
        public int? Tracker()
        {
            using (var tracker = this.ints2.TrackMin())
            {
                Update(this.ints2);
                return tracker.Value;
            }
        }

        private static void Update(ObservableCollection<int> ints)
        {
            if (ints.Count > 1000)
            {
                ints.RemoveAt(ints.Count - 1);
            }
            else
            {
                ints.Add(5);
            }
        }
    }
}
