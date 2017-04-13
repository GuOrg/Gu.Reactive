namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using BenchmarkDotNet.Attributes;

    public class ObservePropertyChangedReact : IDisposable
    {
        private readonly Fake fake = new Fake { Next = new Level { Name = string.Empty } };
        private readonly Fake lambdaFake = new Fake { Next = new Level { Name = string.Empty } };
        private readonly Fake nestedFake = new Fake { Next = new Level { Name = string.Empty } };
        private readonly Fake slimFake = new Fake { Next = new Level { Name = string.Empty } };
        private readonly Fake rxFake = new Fake { Next = new Level { Name = string.Empty } };
        private readonly Fake propertyChangedManagerFake = new Fake { Next = new Level { Name = string.Empty } };

        private readonly CompositeDisposable subscriptions;
        private int count;

        public ObservePropertyChangedReact()
        {
            this.fake.PropertyChanged += this.OnFakeOnPropertyChanged;

            System.ComponentModel.PropertyChangedEventManager.AddHandler(this.propertyChangedManagerFake, this.OnFakeOnPropertyChanged, nameof(Fake.Value));

            this.subscriptions = new CompositeDisposable
                                 {
                                     this.lambdaFake.ObservePropertyChanged(x => x.Value, signalInitial: false)
                                                    .Subscribe(x => this.count++),

                                     this.slimFake.ObservePropertyChangedSlim("Value", signalInitial: false)
                                                  .Subscribe(x => this.count++),

                                     this.nestedFake.ObservePropertyChanged(x => x.Next.Value, signalInitial: false)
                                                .Subscribe(x => this.count++),

                                     Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                             x => this.rxFake.PropertyChanged += x,
                                             x => this.rxFake.PropertyChanged -= x)
                                         .Where(
                                             x =>
                                             string.IsNullOrEmpty(x.EventArgs.PropertyName) ||
                                             x.EventArgs.PropertyName == nameof(this.fake.Value))
                                         .Subscribe(x => this.count++)
                                 };
        }

        [Benchmark(Baseline = true)]
        public int SubscribeToEventStandard()
        {
            this.fake.Value++;
            return this.count;
        }

        [Benchmark]
        public int SimpleLambda()
        {
            this.lambdaFake.Value++;
            return this.count;
        }

        [Benchmark]
        public int Slim()
        {
            this.slimFake.Value++;
            return this.count;
        }

        [Benchmark]
        public int Nested()
        {
            this.nestedFake.Next.Value++;
            return this.count;
        }

        [Benchmark]
        public int Rx()
        {
            this.rxFake.Value++;
            return this.count;
        }

        [Benchmark]
        public int PropertyChangedEventManager()
        {
            this.propertyChangedManagerFake.Value++;
            return this.count;
        }

        public void Dispose()
        {
            this.subscriptions.Dispose();
            this.fake.PropertyChanged -= this.OnFakeOnPropertyChanged;

            System.ComponentModel.PropertyChangedEventManager.RemoveHandler(this.propertyChangedManagerFake, this.OnFakeOnPropertyChanged, nameof(Fake.Value));
        }

        private void OnFakeOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == nameof(this.fake.Value))
            {
                this.count++;
            }
        }
    }
}