namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    internal static class DeferredRefresher
    {
        internal static readonly NotifyCollectionChangedEventArgs NotifyCollectionResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

        internal static IObservable<IReadOnlyList<NotifyCollectionChangedEventArgs>> Create(
            IRefresher refresher,
            IEnumerable source,
            TimeSpan throttleTime,
            IScheduler scheduler,
            bool signalInitial)
        {
            var incc = source as INotifyCollectionChanged;
            if (incc == null)
            {
                return Observable.Empty<IReadOnlyList<NotifyCollectionChangedEventArgs>>();
            }

            var observable = Observable.Create<NotifyCollectionChangedEventArgs>(o =>
            {
                NotifyCollectionChangedEventHandler fsHandler = (_, e) =>
                {
                    if (!refresher.IsRefreshing)
                    {
                        o.OnNext(e);
                    }
                };
                incc.CollectionChanged += fsHandler;
                return Disposable.Create(() => incc.CollectionChanged -= fsHandler);
            });
            return observable.Buffer(throttleTime, scheduler, signalInitial);
        }

        private static IObservable<IReadOnlyList<NotifyCollectionChangedEventArgs>> Buffer(
            this IObservable<NotifyCollectionChangedEventArgs> observable,
            TimeSpan throttleTime,
            IScheduler scheduler,
            bool signalInitial)
        {
            observable = observable.StartWithIf(signalInitial, scheduler, NotifyCollectionResetEventArgs);

            if (throttleTime > TimeSpan.Zero)
            {
                var shared = observable.Publish()
                                       .RefCount();
                return shared.Buffer(() => shared.ThrottleOrDefault(throttleTime, scheduler))
                             .Cast<IReadOnlyList<NotifyCollectionChangedEventArgs>>();
            }

            return observable.Select(x => new[] { x });
        }

        public static IObservable<T> StartWithIf<T>(this IObservable<T> observable, bool condition, IScheduler scheduler, T toPrepend)
        {
            if (!condition)
            {
                return observable;
            }

            if (scheduler != null)
            {
                return observable.StartWith(scheduler, toPrepend);
            }
            return observable.StartWith(toPrepend);
        }

    }
}