namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.ObjectModel;
    using BenchmarkDotNet.Attributes;

    public class ObserveItemPropertyChanged
    {
        private readonly ObservableCollection<Fake> source = new ObservableCollection<Fake>();

        [Setup]
        public void SetupData()
        {
            this.source.Clear();
            for (int i = -5; i < 10; i++)
            {
                this.source.Add(new Fake { Value = i });
            }
        }

        [Benchmark]
        public int ObserveItemPropertyChangedSlimSimpleLambdaAddOne()
        {
            int count = 0;
            using (this.source.ObserveItemPropertyChangedSlim(x => x.Value)
                         .Subscribe(_ => count++))
            {
                this.source.Add(new Fake { Value = 0 });
                return count;
            }
        }

        [Benchmark]
        public int ObserveItemPropertyChangedSlimThreeLevelLambdaAddOne()
        {
            int count = 0;
            using (this.source.ObserveItemPropertyChangedSlim(x => x.Next.Value)
                         .Subscribe(_ => count++))
            {
                this.source.Add(new Fake { Value = 0 });
                return count;
            }
        }

        [Benchmark]
        public int ObserveItemPropertyChangedSlimThreeLevelLambdaAddOneThenUpdate()
        {
            int count = 0;
            using (this.source.ObserveItemPropertyChangedSlim(x => x.Next.Next.Value)
                         .Subscribe(_ => count++))
            {
                var fake = new Fake();
                this.source.Add(fake);
                fake.Next = new Level { Next = new Level { Value = 1 } };
                return count;
            }
        }

        [Benchmark]
        public int ObserveItemPropertyChangedSimpleLambdaAddOne()
        {
            int count = 0;
            using (this.source.ObserveItemPropertyChanged(x => x.Value)
                         .Subscribe(_ => count++))
            {
                this.source.Add(new Fake { Value = 0 });
                return count;
            }
        }

        [Benchmark]
        public int ObserveItemPropertyChangedThreeLevelLambdaAddOne()
        {
            int count = 0;
            using (this.source.ObserveItemPropertyChanged(x => x.Next.Next.Value)
                         .Subscribe(_ => count++))
            {
                this.source.Add(new Fake { Value = 0 });
                return count;
            }
        }

        [Benchmark]
        public int ObserveItemPropertyChangedThreeLevelLambdaAddOneThenUpdate()
        {
            int count = 0;
            using (this.source.ObserveItemPropertyChanged(x => x.Next.Next.Value)
                         .Subscribe(_ => count++))
            {
                var fake = new Fake();
                this.source.Add(fake);
                fake.Next = new Level { Next = new Level { Value = 1 } };
                return count;
            }
        }
    }
}
