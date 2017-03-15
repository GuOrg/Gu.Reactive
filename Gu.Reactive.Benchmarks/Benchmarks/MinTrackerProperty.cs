namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    public class MinTrackerProperty
    {
        private readonly ObservableCollection<Fake> fakes1 = new ObservableCollection<Fake>();
        private readonly ObservableCollection<Fake> fakes2 = new ObservableCollection<Fake>();
        private readonly ObservableCollection<Fake> fakes3 = new ObservableCollection<Fake>();

        [Setup]
        public void SetupData()
        {
            foreach (var fakes in new[] { this.fakes1, this.fakes2, this.fakes3 })
            {
                fakes.Clear();
                for (int i = -5; i < 10; i++)
                {
                    fakes.Add(new Fake { Value = i });
                }
            }
        }

        [Benchmark(Baseline = true)]
        public int? Linq()
        {
            var min = this.fakes1.Min(x => x.Value);
            this.fakes1.Add(new Fake());
            return Math.Min(min, this.fakes1.Min(x => x.Value));
        }

        [Benchmark]
        public int? Tracker()
        {
            using (var tracker = this.fakes2.TrackMin(x => x.Value, -1, false))
            {
                this.fakes2.Add(new Fake { Value = 5 });
                return tracker.Value;
            }
        }

        [Benchmark]
        public int? TrackerChanges()
        {
            using (var changeTracker = this.fakes3.TrackMin(x => x.Value, -1, true))
            {
                this.fakes3.Add(new Fake { Value = 5 });
                return changeTracker.Value;
            }
        }
    }
}