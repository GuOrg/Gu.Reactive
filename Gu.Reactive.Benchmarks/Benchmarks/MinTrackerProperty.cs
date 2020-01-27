namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    public class MinTrackerProperty
    {
        private readonly ObservableCollection<Fake> fakes1 = new ObservableCollection<Fake>();
        private readonly ObservableCollection<Fake> fakes2 = new ObservableCollection<Fake>();
        private readonly ObservableCollection<Fake> fakes3 = new ObservableCollection<Fake>();

        [GlobalSetup]
        public void SetupData()
        {
            foreach (var fakes in new[] { this.fakes1, this.fakes2, this.fakes3 })
            {
                fakes.Clear();
                for (var i = -5; i < 10; i++)
                {
                    fakes.Add(new Fake { Value = i });
                }
            }
        }

        [Benchmark(Baseline = true)]
        public int? Linq()
        {
            var min = this.fakes1.Min(x => x.Value);
            Update(this.fakes1);
            return Math.Min(min, this.fakes1.Min(x => x.Value));
        }

        [Benchmark]
        public int? Tracker()
        {
            using var tracker = this.fakes2.TrackMin(x => x.Value);
            Update(this.fakes2);
            return tracker.Value;
        }

        [Benchmark]
        public int? TrackerChanges()
        {
            using var changeTracker = this.fakes3.TrackMin(x => x.Value);
            Update(this.fakes3);
            return changeTracker.Value;
        }

        private static void Update(ObservableCollection<Fake> ints)
        {
            if (ints.Count > 1000)
            {
                ints.RemoveAt(ints.Count - 1);
            }
            else
            {
                ints.Add(new Fake { Value = 5 });
            }
        }
    }
}
