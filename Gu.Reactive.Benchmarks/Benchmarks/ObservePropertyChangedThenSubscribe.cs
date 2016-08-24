namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;

    using BenchmarkDotNet.Attributes;

    using Gu.Reactive.PropertyPathStuff;

    public class ObservePropertyChangedThenSubscribe
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
        public IDisposable SimpleLamda()
        {
            using (var disposable = _fake.ObservePropertyChanged(x => x.Value, false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable SimpleString()
        {
            using (var disposable = _fake.ObservePropertyChanged("Value", false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable SimpleSlim()
        {
            using (var disposable = _fake.ObservePropertyChangedSlim("Value", false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable NestedCachedPath()
        {
            using (var disposable = _fake.ObservePropertyChanged(_propertyPath, false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable NestedLambda()
        {
            using (var disposable = _fake.ObservePropertyChanged(x => x.Next.Name, false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable Rx()
        {
            using (var disposable = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(x => _fake.PropertyChanged += x, x => _fake.PropertyChanged -= x)
                                              .Where(x => string.IsNullOrEmpty(x.EventArgs.PropertyName) || x.EventArgs.PropertyName == nameof(_fake.Value))
                                              .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public EventHandler<PropertyChangedEventArgs> PropertyChangedEventManager()
        {
            EventHandler<PropertyChangedEventArgs> handler = (sender, args) => {};
            System.ComponentModel.PropertyChangedEventManager.AddHandler(_fake, handler, nameof(_fake.Value));
            System.ComponentModel.PropertyChangedEventManager.RemoveHandler(_fake, handler, nameof(_fake.Value));
            return handler;
        }
    }
}