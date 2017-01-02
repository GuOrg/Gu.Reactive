namespace Gu.Reactive.Benchmarks
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    public class MinTrackerProperty
    {
        private ObservableCollection<Fake> fakes;
        private MinTracker<int> tracker;
        private MinTracker<int> changeTracker;

        [Setup]
        public void SetupData()
        {
            this.fakes = new ObservableCollection<Fake>(Enumerable.Range(-5, 10).Select(x => new Fake { Value = x }));
            this.tracker?.Dispose();
            this.tracker = this.fakes.TrackMin(x => x.Value, -1, false);
            this.changeTracker?.Dispose();
            this.changeTracker = this.fakes.TrackMin(x => x.Value, -1, true);
        }

        [Benchmark(Baseline = true)]
        public int? Linq()
        {
            this.fakes.Add(new Fake());
            return this.fakes.Min(x => x.Value);
        }

        [Benchmark]
        public int? Tracker()
        {
            this.fakes.Add(new Fake { Value = 5 });
            return this.tracker.Value;
        }

        [Benchmark]
        public int? TrackerChanges()
        {
            this.fakes.Add(new Fake { Value = 5 });
            return this.changeTracker.Value;
        }
    }
}