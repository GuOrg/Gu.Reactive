namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;

    using BenchmarkDotNet.Attributes;

    using Gu.Reactive.Internals;

    public class ObservePropertyChangedThenSubscribe
    {
        private readonly Fake fake = new Fake { IsTrue = false, Next = new Level { Name = string.Empty } };

        private readonly NotifyingPath<Fake, string> propertyPath = NotifyingPath.GetOrCreate<Fake, string>(x => x.Next.Name);

        [Benchmark(Baseline = true)]
        public int SubscribeToEventStandard()
        {
            int count = 0;

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
        public IDisposable ObservePropertyChangedSimpleLamda()
        {
            using (var disposable = this.fake.ObservePropertyChanged(x => x.Value, signalInitial: false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable ObservePropertyChangedNestedCachedPath()
        {
            using (var disposable = this.fake.ObservePropertyChanged(this.propertyPath, signalInitial: false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable ObservePropertyChangedNestedLambda()
        {
            using (var disposable = this.fake.ObservePropertyChanged(x => x.Next.Name, signalInitial: false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable ObservePropertyChangedString()
        {
            using (var disposable = this.fake.ObservePropertyChanged("Value", signalInitial: false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable ObservePropertyChangedSlimString()
        {
            using (var disposable = this.fake.ObservePropertyChangedSlim("Value", signalInitial: false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable ObservePropertyChangedSlimSimpleLambda()
        {
            using (var disposable = this.fake.ObservePropertyChangedSlim(x => x.Name, signalInitial: false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable ObservePropertyChangedSlimNestedLambda()
        {
            using (var disposable = this.fake.ObservePropertyChangedSlim(x => x.Next.Name, signalInitial: false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public IDisposable ObservableFromEventPattern()
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
            void Handler(object sender, PropertyChangedEventArgs args)
            {
            }

            System.ComponentModel.PropertyChangedEventManager.AddHandler(this.fake, Handler, nameof(this.fake.Value));
            System.ComponentModel.PropertyChangedEventManager.RemoveHandler(this.fake, Handler, nameof(this.fake.Value));
            return Handler;
        }
    }
}