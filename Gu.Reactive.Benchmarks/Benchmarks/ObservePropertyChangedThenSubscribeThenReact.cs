namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;

    using BenchmarkDotNet.Attributes;

    using Gu.Reactive.PropertyPathStuff;

    public class ObservePropertyChangedThenSubscribeThenReact
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
        public int SimpleLambda()
        {
            int count = 0;
            using (_fake.ObservePropertyChanged(x => x.Value, false).Subscribe(x => count++))
            {
                _fake.Value++;
                return count;
            }
        }

        [Benchmark]
        public int Slim()
        {
            int count = 0;
            using (_fake.ObservePropertyChangedSlim("Value", false).Subscribe(x => count++))
            {
                _fake.Value++;
                return count;
            }
        }

        [Benchmark]
        public int Nested()
        {
            int count = 0;
            using (_fake.ObservePropertyChanged(x => x.Next.Value, false)
                        .Subscribe(x => count++))
            {
                _fake.Next.Value++;
                return count;
            }
        }

        [Benchmark]
        public int NestedCachedPath()
        {
            int count = 0;
            using (_fake.ObservePropertyChanged(_propertyPath, false)
                        .Subscribe(x => count++))
            {
                _fake.Next.Value++;
                return count;
            }
        }

        [Benchmark]
        public int Rx()
        {
            int count = 0;
            using (Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(x => _fake.PropertyChanged += x, x => _fake.PropertyChanged -= x)
                             .Where(x => string.IsNullOrEmpty(x.EventArgs.PropertyName) || x.EventArgs.PropertyName == nameof(_fake.Value))
                             .Subscribe(x => count++))
            {
                _fake.Value++;
                return count;
            }
        }

        [Benchmark]
        public int PropertyChangedEventManager()
        {
            int count = 0;
            EventHandler<PropertyChangedEventArgs> handler = (sender, args) => count++;
            System.ComponentModel.PropertyChangedEventManager.AddHandler(_fake, handler, nameof(_fake.Value));
            _fake.Value++;
            System.ComponentModel.PropertyChangedEventManager.RemoveHandler(_fake, handler, nameof(_fake.Value));
            return count;
        }
    }
}