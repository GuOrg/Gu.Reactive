namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    using BenchmarkDotNet.Attributes;

    using Gu.Reactive.PropertyPathStuff;

    public class ObservePropertyChangedReact : IDisposable
    {
        private readonly Fake _fake = new Fake { Next = new Level { Name = "" } };
        private readonly Fake _lambdaFake = new Fake { Next = new Level { Name = "" } };
        private readonly Fake _nestedFake = new Fake { Next = new Level { Name = "" } };
        private readonly Fake _slimFake = new Fake { Next = new Level { Name = "" } };
        private readonly Fake _rxFake = new Fake { Next = new Level { Name = "" } };
        private readonly Fake _propertyChangedManagerFake = new Fake { Next = new Level { Name = "" } };

        private readonly PropertyPath<Fake, string> _propertyPath = PropertyPathStuff.PropertyPath.Create<Fake, string>(x => x.Next.Name);
        private readonly CompositeDisposable _subscriptions;
        private int _count;

        public ObservePropertyChangedReact()
        {
            _fake.PropertyChanged += OnFakeOnPropertyChanged;

            System.ComponentModel.PropertyChangedEventManager.AddHandler(_propertyChangedManagerFake, OnFakeOnPropertyChanged, nameof(Fake.Value));

            _subscriptions = new CompositeDisposable
                                 {
                                     _lambdaFake.ObservePropertyChanged(x => x.Value, false)
                                                .Subscribe(x => _count++),

                                     _slimFake.ObservePropertyChangedSlim("Value", false).Subscribe(x => _count++),

                                     _nestedFake.ObservePropertyChanged(x => x.Next.Value, false)
                                                .Subscribe(x => _count++),

                                     Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                             x => _rxFake.PropertyChanged += x,
                                             x => _rxFake.PropertyChanged -= x)
                                         .Where(
                                             x =>
                                             string.IsNullOrEmpty(x.EventArgs.PropertyName) ||
                                             x.EventArgs.PropertyName == nameof(_fake.Value))
                                         .Subscribe(x => _count++)
                                 };
        }

        [Benchmark(Baseline = true)]
        public int SubscribeToEventStandard()
        {
            _fake.Value++;
            return _count;
        }

        [Benchmark]
        public int SimpleLambda()
        {
            _lambdaFake.Value++;
            return _count;
        }

        [Benchmark]
        public int Slim()
        {
            _slimFake.Value++;
            return _count;
        }

        [Benchmark]
        public int Nested()
        {
            _nestedFake.Next.Value++;
            return _count;
        }

        [Benchmark]
        public int Rx()
        {
            _rxFake.Value++;
            return _count;
        }

        [Benchmark]
        public int PropertyChangedEventManager()
        {
            _propertyChangedManagerFake.Value++;
            return _count;
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
            _fake.PropertyChanged -= OnFakeOnPropertyChanged;

            System.ComponentModel.PropertyChangedEventManager.RemoveHandler(_propertyChangedManagerFake, OnFakeOnPropertyChanged, nameof(Fake.Value));
        }

        private void OnFakeOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == nameof(_fake.Value))
            {
                _count++;
            }
        }
    }
}