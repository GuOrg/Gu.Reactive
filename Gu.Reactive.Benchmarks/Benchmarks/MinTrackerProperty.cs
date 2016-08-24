namespace Gu.Reactive.Benchmarks
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    public class MinTrackerProperty
    {
        private ObservableCollection<Fake> _fakes;
        private MinTracker<int> _tracker;
        private MinTracker<int> _changeTracker;

        [Setup]
        public void SetupData()
        {
            _fakes = new ObservableCollection<Fake>(Enumerable.Range(-5, 10).Select(x => new Fake { Value = x }));
            _tracker?.Dispose();
            _tracker = _fakes.TrackMin(x => x.Value, -1, false);
            _changeTracker?.Dispose();
            _changeTracker = _fakes.TrackMin(x => x.Value, -1, true);
        }

        [Benchmark(Baseline = true)]
        public int? Linq()
        {
            _fakes.Add(new Fake());
            return _fakes.Min(x => x.Value);
        }

        [Benchmark]
        public int? Tracker()
        {
            _fakes.Add(new Fake { Value = 5 });
            return _tracker.Value;
        }

        [Benchmark]
        public int? TrackerChanges()
        {
            _fakes.Add(new Fake { Value = 5 });
            return _changeTracker.Value;
        }
    }
}