namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;

    using BenchmarkDotNet.Attributes;

    using Gu.Reactive.PropertyPathStuff;

    public class ObservePropertyChangedThenSubscribe
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
        public IDisposable SimpleLamda()
        {
            using (var disposable = this.fake.ObservePropertyChanged(x => x.Value, false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable SimpleString()
        {
            using (var disposable = this.fake.ObservePropertyChanged("Value", false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable SimpleSlim()
        {
            using (var disposable = this.fake.ObservePropertyChangedSlim("Value", false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable NestedCachedPath()
        {
            using (var disposable = this.fake.ObservePropertyChanged(this.propertyPath, false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable NestedLambda()
        {
            using (var disposable = this.fake.ObservePropertyChanged(x => x.Next.Name, false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable Rx()
        {
            using (var disposable = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(x => this.fake.PropertyChanged += x, x => this.fake.PropertyChanged -= x)
                                              .Where(x => string.IsNullOrEmpty(x.EventArgs.PropertyName) || x.EventArgs.PropertyName == nameof(this.fake.Value))
                                              .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public EventHandler<PropertyChangedEventArgs> PropertyChangedEventManager()
        {
            EventHandler<PropertyChangedEventArgs> handler = (sender, args) => { };
            System.ComponentModel.PropertyChangedEventManager.AddHandler(this.fake, handler, nameof(this.fake.Value));
            System.ComponentModel.PropertyChangedEventManager.RemoveHandler(this.fake, handler, nameof(this.fake.Value));
            return handler;
        }
    }
}