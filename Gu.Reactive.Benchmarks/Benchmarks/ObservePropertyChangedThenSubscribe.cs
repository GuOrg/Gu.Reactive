#pragma warning disable IDISP011 // Don't return diposed instance.
namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using BenchmarkDotNet.Attributes;

    using Gu.Reactive.Internals;

    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    public class ObservePropertyChangedThenSubscribe
    {
        private static readonly Subject<int> Subject = new Subject<int>();
        private static readonly Fake Fake = new Fake { IsTrue = false, Next = new Level { Name = string.Empty } };

        private static readonly NotifyingPath<Fake, string> PropertyPath = NotifyingPath.GetOrCreate<Fake, string>(x => x.Next.Name);

        [Benchmark(Baseline = true)]
        public static int SubscribeToEventStandard()
        {
            var count = 0;
            void Handler(object sender, PropertyChangedEventArgs args)
            {
                if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == nameof(Fake.Value))
                {
                    count++;
                }
            }

            Fake.PropertyChanged += Handler;
            Fake.Value++;
            Fake.PropertyChanged -= Handler;
            return count;
        }

        [Benchmark]
        public static IDisposable SubjectSubscribe()
        {
            using (var disposable = Subject.Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public static object SimpleLambda()
        {
            Expression<Func<Fake, int>> expression = x => x.Value;
            return expression;
        }

        [Benchmark]
        public static object NestedLambda()
        {
            Expression<Func<Fake, int>> expression = x => x.Next.Value;
            return expression;
        }

        [Benchmark]
        public static IDisposable ObservePropertyChangedSimpleLamda()
        {
            using (var disposable = Fake.ObservePropertyChanged(x => x.Value, signalInitial: false)
                                        .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public static IDisposable ObservePropertyChangedNestedCachedPath()
        {
            using (var disposable = Fake.ObservePropertyChanged(PropertyPath, signalInitial: false)
                                        .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public static IDisposable ObservePropertyChangedNestedLambda()
        {
            using (var disposable = Fake.ObservePropertyChanged(x => x.Next.Name, signalInitial: false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public static IDisposable ObservePropertyChangedString()
        {
            using (var disposable = Fake.ObservePropertyChanged("Value", signalInitial: false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public static IDisposable ObservePropertyChangedSlimString()
        {
            using (var disposable = Fake.ObservePropertyChangedSlim("Value", signalInitial: false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public static IDisposable ObservePropertyChangedSlimSimpleLambda()
        {
            using (var disposable = Fake.ObservePropertyChangedSlim(x => x.Name, signalInitial: false)
                                        .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public static IDisposable ObservePropertyChangedSlimNestedLambda()
        {
            using (var disposable = Fake.ObservePropertyChangedSlim(x => x.Next.Name, signalInitial: false)
                                         .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public static IDisposable ObservableFromEventPattern()
        {
            using (var disposable = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(x => Fake.PropertyChanged += x, x => Fake.PropertyChanged -= x)
                                              .Where(x => string.IsNullOrEmpty(x.EventArgs.PropertyName) || x.EventArgs.PropertyName == nameof(Fake.Value))
                                              .Subscribe(_ => { }))
            {
                return disposable;
            }
        }

        [Benchmark]
        public static EventHandler<PropertyChangedEventArgs> PropertyChangedEventManager()
        {
            void Handler(object sender, PropertyChangedEventArgs args)
            {
            }

            System.ComponentModel.PropertyChangedEventManager.AddHandler(Fake, Handler, nameof(Fake.Value));
            System.ComponentModel.PropertyChangedEventManager.RemoveHandler(Fake, Handler, nameof(Fake.Value));
            return Handler;
        }
    }
}
