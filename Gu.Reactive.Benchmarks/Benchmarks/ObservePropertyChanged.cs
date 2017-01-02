namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Reactive;

    using BenchmarkDotNet.Attributes;

    using Gu.Reactive.PropertyPathStuff;

    public class ObservePropertyChanged
    {
        private readonly Fake fake = new Fake { IsTrue = false, Next = new Level { Name = string.Empty } };

        private readonly PropertyPath<Fake, string> propertyPath = PropertyPathStuff.PropertyPath.Create<Fake, string>(x => x.Next.Name);

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
        public IObservable<EventPattern<PropertyChangedEventArgs>> SimpleLambda()
        {
            return this.fake.ObservePropertyChanged(x => x.Value, false);
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> SimpleString()
        {
            return this.fake.ObservePropertyChanged("Value", false);
        }

        [Benchmark]
        public IObservable<PropertyChangedEventArgs> SimpleSlim()
        {
            return this.fake.ObservePropertyChangedSlim("Value", false);
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> NestedLambda()
        {
            return this.fake.ObservePropertyChanged(x => x.Next.Name, false);
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> NestedCachedPath()
        {
            return this.fake.ObservePropertyChanged(this.propertyPath, false);
        }
    }
}
