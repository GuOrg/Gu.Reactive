namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.ObjectModel;
    using BenchmarkDotNet.Attributes;

    public class ObserveItemPropertyChanged
    {
        [Benchmark]
        public int AddSimple()
        {
            int count = 0;
            var source = new ObservableCollection<Fake>();
            source.ObserveItemPropertyChanged(x => x.Value)
                  .Subscribe(_ => count++);
            source.Add(new Fake());
            return count;
        }

        [Benchmark]
        public int AddNested()
        {
            int count = 0;
            var source = new ObservableCollection<Fake>();
            source.ObserveItemPropertyChanged(x => x.Next.Next.Value)
                  .Subscribe(_ => count++);
            source.Add(new Fake());
            return count;
        }

        [Benchmark]
        public int AddNestedThatUpdates()
        {
            int count = 0;
            var source = new ObservableCollection<Fake>();
            source.ObserveItemPropertyChanged(x => x.Next.Next.Value)
                  .Subscribe(_ => count++);
            var fake = new Fake();
            source.Add(fake);
            fake.Next = new Level { Next = new Level { Value = 1 } };
            return count;
        }
    }
}
