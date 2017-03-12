namespace Gu.Reactive.Benchmarks
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    public class MinTrackerProperty
    {
        private readonly ObservableCollection<Fake> fakes = new ObservableCollection<Fake>();

        [Setup]
        public void SetupData()
        {
            this.fakes.Clear();
            for (int i = -5; i < 10; i++)
            {
                this.fakes.Add(new Fake { Value = i });
            }
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
            using (var tracker = this.fakes.TrackMin(x => x.Value, -1, false))
            {
                this.fakes.Add(new Fake { Value = 5 });
                return tracker.Value;
            }
        }

        [Benchmark]
        public int? TrackerChanges()
        {
            using (var changeTracker = this.fakes.TrackMin(x => x.Value, -1, true))
            {
                this.fakes.Add(new Fake { Value = 5 });
                return changeTracker.Value;
            }
        }
    }
}