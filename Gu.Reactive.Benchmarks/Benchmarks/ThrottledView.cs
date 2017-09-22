#pragma warning disable 618
namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.ObjectModel;
    using BenchmarkDotNet.Attributes;
    using Gu.Wpf.Reactive;

    public class ThrottledView : IDisposable
    {
        // ReSharper disable once CollectionNeverQueried.Local
        private readonly ObservableCollection<int> reference = new ObservableCollection<int>();
        private readonly ObservableCollection<int> ints = new ObservableCollection<int>();
        private readonly ThrottledView<int> view;
        private bool disposed;

        public ThrottledView()
        {
            this.view = this.ints.AsThrottledView(TimeSpan.FromMilliseconds(10));
        }

#pragma warning disable WPF1011 // Implement INotifyPropertyChanged.
        [Params(1000)]
        public int N { get; set; }
#pragma warning restore WPF1011 // Implement INotifyPropertyChanged.

        [Setup]
        public void SetupData()
        {
            this.reference.Clear();
            this.ints.Clear();
        }

        [Benchmark(Baseline = true)]
        public void AddToReference()
        {
            for (var i = 0; i < this.N; i++)
            {
                this.reference.Add(i);
            }
        }

        [Benchmark]
        public void AddToSource()
        {
            for (var i = 0; i < this.N; i++)
            {
                this.ints.Add(i);
            }
        }

        [Benchmark]
        public void AddToView()
        {
            for (var i = 0; i < this.N; i++)
            {
                this.view.Add(i);
            }
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.view?.Dispose();
        }
    }
}
