namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.ObjectModel;
    using BenchmarkDotNet.Attributes;

    public class ThrottledView
    {
        private readonly ObservableCollection<int> _reference = new ObservableCollection<int>();
        private readonly ObservableCollection<int> _ints = new ObservableCollection<int>();
        private readonly ThrottledView<int> _view;

        public ThrottledView()
        {
            _view = _ints.AsThrottledView(TimeSpan.FromMilliseconds(10));
        }

        [Params(1000)]
        public int N { get; set; }

        [Setup]
        public void SetupData()
        {
            _reference.Clear();
            _ints.Clear();
        }

        [Benchmark(Baseline = true)]
        public void AddToReference()
        {
            for (int i = 0; i < N; i++)
            {
                _reference.Add(i);
            }
        }

        [Benchmark]
        public void AddToSource()
        {
            for (int i = 0; i < N; i++)
            {
                _ints.Add(i);
            }
        }

        [Benchmark]
        public void AddToView()
        {
            for (int i = 0; i < N; i++)
            {
                _view.Add(i);
            }
        }
    }
}
