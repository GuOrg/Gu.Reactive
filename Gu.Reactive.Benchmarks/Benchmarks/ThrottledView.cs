namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.ObjectModel;
    using BenchmarkDotNet.Attributes;

    public class ThrottledView
    {
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly ObservableCollection<int> reference = new ObservableCollection<int>();
        private readonly ObservableCollection<int> ints = new ObservableCollection<int>();
        private readonly ThrottledView<int> view;

        public ThrottledView()
        {
            this.view = this.ints.AsThrottledView(TimeSpan.FromMilliseconds(10));
        }

        [Params(1000)]
        public int N { get; set; }

        [Setup]
        public void SetupData()
        {
            this.reference.Clear();
            this.ints.Clear();
        }

        [Benchmark(Baseline = true)]
        public void AddToReference()
        {
            for (int i = 0; i < this.N; i++)
            {
                this.reference.Add(i);
            }
        }

        [Benchmark]
        public void AddToSource()
        {
            for (int i = 0; i < this.N; i++)
            {
                this.ints.Add(i);
            }
        }

        [Benchmark]
        public void AddToView()
        {
            for (int i = 0; i < this.N; i++)
            {
                this.view.Add(i);
            }
        }
    }
}
