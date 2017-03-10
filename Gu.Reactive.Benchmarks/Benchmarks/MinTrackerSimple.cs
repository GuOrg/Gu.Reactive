namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    public class MinTrackerSimple : IDisposable
    {
        private ObservableCollection<int> ints;
        private MinTracker<int> tracker;
        private bool disposed;

        [Setup]
        public void SetupData()
        {
            this.ints = new ObservableCollection<int>(Enumerable.Range(-5, 10));
            this.tracker?.Dispose();
            this.tracker = this.ints.TrackMin(-1);
        }

        [Benchmark(Baseline = true)]
        public int? Linq()
        {
            this.ints.Add(5);
            return this.ints.Min(x => x);
        }

        [Benchmark]
        public int? Tracker()
        {
            this.ints.Add(5);
            return this.tracker.Value;
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.tracker?.Dispose();
        }
    }
}