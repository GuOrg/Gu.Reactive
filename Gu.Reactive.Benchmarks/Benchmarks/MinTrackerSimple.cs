namespace Gu.Reactive.Benchmarks
{
    using System.Collections.ObjectModel;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    public class MinTrackerSimple
    {
        private ObservableCollection<int> _ints;
        private MinTracker<int> _tracker;

        [Setup]
        public void SetupData()
        {
            _ints = new ObservableCollection<int>(Enumerable.Range(-5, 10));
            _tracker?.Dispose();
            _tracker = _ints.TrackMin(-1);
        }

        [Benchmark(Baseline = true)]
        public int? Linq()
        {
            _ints.Add(5);
            return _ints.Min(x => x);
        }

        [Benchmark]
        public int? Tracker()
        {
            _ints.Add( 5);
            return _tracker.Value;
        }
    }
}