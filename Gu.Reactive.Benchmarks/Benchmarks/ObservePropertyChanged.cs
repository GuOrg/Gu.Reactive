#pragma warning disable CS8602, CS8603
namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive;

    using BenchmarkDotNet.Attributes;

    using Gu.Reactive.Internals;

    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    public class ObservePropertyChanged
    {
        private readonly Fake fake = new Fake { IsTrue = false, Next = new Level { Name = string.Empty } };

        private readonly NotifyingPath<Fake, string> propertyPath = NotifyingPath.GetOrCreate<Fake, string>(x => x.Next.Name);

        [Benchmark(Baseline = true)]
        public int SubscribeToEventStandard()
        {
            var count = 0;
            void Handler(object sender, PropertyChangedEventArgs args)
            {
                if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == nameof(this.fake.Value))
                {
                    count++;
                }
            }

            this.fake.PropertyChanged += Handler;
            this.fake.Value++;
            this.fake.PropertyChanged -= Handler;
            return count;
        }

        [Benchmark]
        public Expression<Func<Fake, int>> ExpressionSimpleLambda()
        {
            return x => x.Value;
        }

        [Benchmark]
        public Expression<Func<Fake, string>> ExpressionNestedLambda()
        {
            return x => x.Next.Name;
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChangedSimpleLambda()
        {
            return this.fake.ObservePropertyChanged(x => x.Value, signalInitial: false);
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChangedNestedLambda()
        {
            return this.fake.ObservePropertyChanged(x => x.Next.Name, signalInitial: false);
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChangedNestedCachedPath()
        {
            return this.fake.ObservePropertyChanged(this.propertyPath, signalInitial: false);
        }

        [Benchmark]
        public IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChangedString()
        {
            return this.fake.ObservePropertyChanged("Value", signalInitial: false);
        }

        [Benchmark]
        public IObservable<PropertyChangedEventArgs> ObservePropertyChangedSlimString()
        {
            return this.fake.ObservePropertyChangedSlim("Value", signalInitial: false);
        }

        [Benchmark]
        public IObservable<PropertyChangedEventArgs> ObservePropertyChangedSlimSimpleLambda()
        {
            return this.fake.ObservePropertyChangedSlim(x => x.Value, signalInitial: false);
        }

        [Benchmark]
        public IObservable<PropertyChangedEventArgs> ObservePropertyChangedSlimNestedLambda()
        {
            return this.fake.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial: false);
        }
    }
}
