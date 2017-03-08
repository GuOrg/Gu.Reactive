namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;

    using BenchmarkDotNet.Attributes;

    using Gu.Reactive.Internals;

    public class ObservePropertyChangedThenSubscribeThenReact
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
        public int ObservePropertyChangedSimpleLambda()
        {
            int count = 0;
            using (this.fake.ObservePropertyChanged(x => x.Value, false).Subscribe(x => count++))
            {
                this.fake.Value++;
                return count;
            }
        }

        [Benchmark]
        public int ObservePropertyChangedNestedLambda()
        {
            int count = 0;
            using (this.fake.ObservePropertyChanged(x => x.Next.Value, false)
                        .Subscribe(x => count++))
            {
                this.fake.Next.Value++;
                return count;
            }
        }

        [Benchmark]
        public int ObservePropertyChangedNestedCachedPath()
        {
            int count = 0;
            using (this.fake.ObservePropertyChanged(this.propertyPath, false)
                        .Subscribe(x => count++))
            {
                this.fake.Next.Value++;
                return count;
            }
        }

        [Benchmark]
        public int ObservePropertyChangedSlimString()
        {
            int count = 0;
            using (this.fake.ObservePropertyChangedSlim("Value", false).Subscribe(x => count++))
            {
                this.fake.Value++;
                return count;
            }
        }

        [Benchmark]
        public int ObservePropertyChangedSlimSimpleLambda()
        {
            int count = 0;
            using (this.fake.ObservePropertyChangedSlim(x => x.Value, false).Subscribe(x => count++))
            {
                this.fake.Value++;
                return count;
            }
        }

        [Benchmark]
        public int ObservePropertyChangedSlimNestedLambda()
        {
            int count = 0;
            using (this.fake.ObservePropertyChangedSlim(x => x.Next.Value, false)
                        .Subscribe(x => count++))
            {
                this.fake.Next.Value++;
                return count;
            }
        }

        [Benchmark]
        public int ObservableFromEventPattern()
        {
            int count = 0;
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
            int count = 0;
            EventHandler<PropertyChangedEventArgs> handler = (sender, args) => count++;
            System.ComponentModel.PropertyChangedEventManager.AddHandler(this.fake, handler, nameof(this.fake.Value));
            this.fake.Value++;
            System.ComponentModel.PropertyChangedEventManager.RemoveHandler(this.fake, handler, nameof(this.fake.Value));
            return count;
        }
    }
}