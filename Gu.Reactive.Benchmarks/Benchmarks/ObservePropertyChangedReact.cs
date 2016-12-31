namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using BenchmarkDotNet.Attributes;

    public class ObservePropertyChangedReact : IDisposable
    {
        private readonly Fake fake = new Fake { Next = new Level { Name = "" } };
        private readonly Fake lambdaFake = new Fake { Next = new Level { Name = "" } };
        private readonly Fake nestedFake = new Fake { Next = new Level { Name = "" } };
        private readonly Fake slimFake = new Fake { Next = new Level { Name = "" } };
        private readonly Fake rxFake = new Fake { Next = new Level { Name = "" } };
        private readonly Fake propertyChangedManagerFake = new Fake { Next = new Level { Name = "" } };

        private readonly CompositeDisposable subscriptions;
        private int count;

        public ObservePropertyChangedReact()
        {
            fake.PropertyChanged += OnFakeOnPropertyChanged;

            System.ComponentModel.PropertyChangedEventManager.AddHandler(propertyChangedManagerFake, OnFakeOnPropertyChanged, nameof(Fake.Value));

            subscriptions = new CompositeDisposable
                                 {
                                     lambdaFake.ObservePropertyChanged(x => x.Value, false)
                                                .Subscribe(x => count++),

                                     slimFake.ObservePropertyChangedSlim("Value", false).Subscribe(x => count++),

                                     nestedFake.ObservePropertyChanged(x => x.Next.Value, false)
                                                .Subscribe(x => count++),

                                     Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                             x => rxFake.PropertyChanged += x,
                                             x => rxFake.PropertyChanged -= x)
                                         .Where(
                                             x =>
                                             string.IsNullOrEmpty(x.EventArgs.PropertyName) ||
                                             x.EventArgs.PropertyName == nameof(fake.Value))
                                         .Subscribe(x => count++)
                                 };
        }

        [Benchmark(Baseline = true)]
        public int SubscribeToEventStandard()
        {
            fake.Value++;
            return count;
        }

        [Benchmark]
        public int SimpleLambda()
        {
            lambdaFake.Value++;
            return count;
        }

        [Benchmark]
        public int Slim()
        {
            slimFake.Value++;
            return count;
        }

        [Benchmark]
        public int Nested()
        {
            nestedFake.Next.Value++;
            return count;
        }

        [Benchmark]
        public int Rx()
        {
            rxFake.Value++;
            return count;
        }

        [Benchmark]
        public int PropertyChangedEventManager()
        {
            propertyChangedManagerFake.Value++;
            return count;
        }

        public void Dispose()
        {
            subscriptions.Dispose();
            fake.PropertyChanged -= OnFakeOnPropertyChanged;

            System.ComponentModel.PropertyChangedEventManager.RemoveHandler(propertyChangedManagerFake, OnFakeOnPropertyChanged, nameof(Fake.Value));
        }

        private void OnFakeOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == nameof(fake.Value))
            {
                count++;
            }
        }
    }
}