namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Reactive;

    using BenchmarkDotNet.Attributes;

    using Gu.Reactive.PropertyPathStuff;

    public class ObservePropertyChanged
    {
        private readonly Fake _fake = new Fake { IsTrue = false, Next = new Level { Name = "" } };

        private readonly PropertyPath<Fake, string> _propertyPath = PropertyPathStuff.PropertyPath.Create<Fake, string>(x => x.Next.Name);

        [Benchmark(Baseline = true)]
        public int SubscribeToEventStandard()
        {
            int count = 0;
            PropertyChangedEventHandler handler = (sender, args) =>
                {
                    if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == nameof(_fake.Value))
                    {
                        count++;
                    }
                };

            _fake.PropertyChanged += handler;
            _fake.Value++;
            _fake.PropertyChanged -= handler;
            return count;
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> SimpleLambda()
        {
            return _fake.ObservePropertyChanged(x => x.Value, false);
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> SimpleString()
        {
            return _fake.ObservePropertyChanged("Value", false);
        }

        [Benchmark]
        public IObservable<PropertyChangedEventArgs> SimpleSlim()
        {
            return _fake.ObservePropertyChangedSlim("Value", false);
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> NestedLambda()
        {
            return _fake.ObservePropertyChanged(x => x.Next.Name, false);
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> NestedCachedPath()
        {
            return _fake.ObservePropertyChanged(_propertyPath, false);
        }
    }
}
