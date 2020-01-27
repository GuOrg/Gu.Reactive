#pragma warning disable CS8602, CS8603
namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;

    using BenchmarkDotNet.Attributes;

    using Gu.Reactive.Internals;

    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    public class ObservePropertyChangedThenSubscribeThenReact
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
        public int ObservePropertyChangedSimpleLambda()
        {
            var count = 0;
            using (this.fake.ObservePropertyChanged(x => x.Value, signalInitial: false).Subscribe(x => count++))
            {
                this.fake.Value++;
                return count;
            }
        }

        [Benchmark]
        public int ObservePropertyChangedNestedLambda()
        {
            var count = 0;
            using (this.fake.ObservePropertyChanged(x => x.Next.Value, signalInitial: false)
                        .Subscribe(x => count++))
            {
                this.fake.Next.Value++;
                return count;
            }
        }

        [Benchmark]
        public int ObservePropertyChangedNestedCachedPath()
        {
            var count = 0;
            using (this.fake.ObservePropertyChanged(this.propertyPath, signalInitial: false)
                        .Subscribe(x => count++))
            {
                this.fake.Next.Value++;
                return count;
            }
        }

        [Benchmark]
        public int ObservePropertyChangedSlimString()
        {
            var count = 0;
            using (this.fake.ObservePropertyChangedSlim("Value", signalInitial: false)
                            .Subscribe(x => count++))
            {
                this.fake.Value++;
                return count;
            }
        }

        [Benchmark]
        public int ObservePropertyChangedSlimSimpleLambda()
        {
            var count = 0;
            using (this.fake.ObservePropertyChangedSlim(x => x.Value, signalInitial: false)
                            .Subscribe(x => count++))
            {
                this.fake.Value++;
                return count;
            }
        }

        [Benchmark]
        public int ObservePropertyChangedSlimNestedLambda()
        {
            var count = 0;
            using (this.fake.ObservePropertyChangedSlim(x => x.Next.Value, signalInitial: false)
                        .Subscribe(x => count++))
            {
                this.fake.Next.Value++;
                return count;
            }
        }

        [Benchmark]
        public int ObservableFromEventPattern()
        {
            var count = 0;
            using (Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                x => this.fake.PropertyChanged += x,
                x => this.fake.PropertyChanged -= x)
                             .Where(x => string.IsNullOrEmpty(x.EventArgs.PropertyName) || x.EventArgs.PropertyName == nameof(this.fake.Value))
                             .Subscribe(x => count++))
            {
                this.fake.Value++;
                return count;
            }
        }

        [Benchmark]
        public int PropertyChangedEventManager()
        {
            var count = 0;
            void Handler(object sender, PropertyChangedEventArgs args) => count++;
            System.ComponentModel.PropertyChangedEventManager.AddHandler(this.fake, Handler, nameof(this.fake.Value));
            this.fake.Value++;
            System.ComponentModel.PropertyChangedEventManager.RemoveHandler(this.fake, Handler, nameof(this.fake.Value));
            return count;
        }
    }
}
