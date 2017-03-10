namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.ObjectModel;
    using BenchmarkDotNet.Attributes;

    public class ObserveItemPropertyChanged
    {
        [Benchmark]
        public int ObserveItemPropertyChangedSlimSimpleLambdaAddOne()
        {
            int count = 0;
            var source = new ObservableCollection<Fake>();
            using (source.ObserveItemPropertyChangedSlim(x => x.Value)
                         .Subscribe(_ => count++))
            {
                source.Add(new Fake());
                return count;
            }
        }

        [Benchmark]
        public int ObserveItemPropertyChangedSlimThreeLevelLambdaAddOne()
        {
            int count = 0;
            var source = new ObservableCollection<Fake>();
            using (source.ObserveItemPropertyChangedSlim(x => x.Next.Next.Value)
                         .Subscribe(_ => count++))
            {
                source.Add(new Fake());
                return count;
            }
        }

        [Benchmark]
        public int ObserveItemPropertyChangedSlimThreeLevelLambdaAddOneThenUpdate()
        {
            int count = 0;
            var source = new ObservableCollection<Fake>();
            using (source.ObserveItemPropertyChangedSlim(x => x.Next.Next.Value)
                         .Subscribe(_ => count++))
            {
                var fake = new Fake();
                source.Add(fake);
                fake.Next = new Level { Next = new Level { Value = 1 } };
                return count;
            }
        }

        [Benchmark]
        public int ObserveItemPropertyChangedSimpleLambdaAddOne()
        {
            int count = 0;
            var source = new ObservableCollection<Fake>();
            using (source.ObserveItemPropertyChanged(x => x.Value)
                         .Subscribe(_ => count++))
            {
                source.Add(new Fake());
                return count;
            }
        }

        [Benchmark]
        public int ObserveItemPropertyChangedThreeLevelLambdaAddOne()
        {
            int count = 0;
            var source = new ObservableCollection<Fake>();
            using (source.ObserveItemPropertyChanged(x => x.Next.Next.Value)
                         .Subscribe(_ => count++))
            {
                source.Add(new Fake());
                return count;
            }
        }

        [Benchmark]
        public int ObserveItemPropertyChangedThreeLevelLambdaAddOneThenUpdate()
        {
            int count = 0;
            var source = new ObservableCollection<Fake>();
            using (source.ObserveItemPropertyChanged(x => x.Next.Next.Value)
                         .Subscribe(_ => count++))
            {
                var fake = new Fake();
                source.Add(fake);
                fake.Next = new Level { Next = new Level { Value = 1 } };
                return count;
            }
        }
    }
}
