namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Reactive;

    using BenchmarkDotNet.Attributes;

    using Gu.Reactive.Internals;

    public class ObservePropertyChanged
    {
        private readonly Fake fake = new Fake { IsTrue = false, Next = new Level { Name = string.Empty } };

        private readonly NotifyingPath<Fake, string> propertyPath = NotifyingPath.GetOrCreate<Fake, string>(x => x.Next.Name);

        [Benchmark(Baseline = true)]
        public int SubscribeToEventStandard()
        {
            int count = 0;
            PropertyChangedEventHandler handler = (sender, args) =>
                {
                    if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == nameof(this.fake.Value))
                    {
                        count++;
                    }
                };

            this.fake.PropertyChanged += handler;
            this.fake.Value++;
            this.fake.PropertyChanged -= handler;
            return count;
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChangedSimpleLambda()
        {
            return this.fake.ObservePropertyChanged(x => x.Value, false);
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChangedNestedLambda()
        {
            return this.fake.ObservePropertyChanged(x => x.Next.Name, false);
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChangedNestedCachedPath()
        {
            return this.fake.ObservePropertyChanged(this.propertyPath, false);
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChangedString()
        {
            return this.fake.ObservePropertyChanged("Value", false);
        }

        [Benchmark]
        public IObservable<PropertyChangedEventArgs> ObservePropertyChangedSlimString()
        {
            return this.fake.ObservePropertyChangedSlim("Value", false);
        }

        [Benchmark]
        public IObservable<PropertyChangedEventArgs> ObservePropertyChangedSlimSimpleLambda()
        {
            return this.fake.ObservePropertyChangedSlim(x => x.Value, false);
        }

        [Benchmark]
        public IObservable<PropertyChangedEventArgs> ObservePropertyChangedSlimNestedLambda()
        {
            return this.fake.ObservePropertyChangedSlim(x => x.Next.Value, false);
        }
    }
}
