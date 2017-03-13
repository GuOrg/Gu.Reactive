namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;

    internal static class ThrottledRefresher
    {
        private static readonly IObservable<IReadOnlyList<NotifyCollectionChangedEventArgs>> Empty = Observable.Empty<IReadOnlyList<NotifyCollectionChangedEventArgs>>();

        internal static IObservable<IReadOnlyList<NotifyCollectionChangedEventArgs>> Create(
            IUpdater updater,
            IEnumerable source,
            TimeSpan throttleTime,
            IScheduler scheduler,
            bool signalInitial)
        {
            var incc = source as INotifyCollectionChanged;
            if (incc == null)
            {
                return Empty;
            }

            var observable = Observable.Create<NotifyCollectionChangedEventArgs>(o =>
                {
                    NotifyCollectionChangedEventHandler handler = (_, e) =>
                        {
                            var isUpdatingSourceItem = updater.CurrentlyUpdatingSourceItem;
                            if (isUpdatingSourceItem == null)
                            {
                                o.OnNext(e);
                                return;
                            }

                            if (e.TryGetSingleNewItem(out object newItem) &&
                                ReferenceEquals(isUpdatingSourceItem, newItem))
                            {
                                return;
                            }

                            if (e.TryGetSingleOldItem(out object oldItem) && 
                                ReferenceEquals(isUpdatingSourceItem, oldItem))
                            {
                                return;
                            }

                            o.OnNext(e);
                        };
                    incc.CollectionChanged += handler;
                    return Disposable.Create(() => incc.CollectionChanged -= handler);
                });
            return observable.Buffer(throttleTime, scheduler, signalInitial);
        }

        internal static IObservable<T> StartWithIf<T>(this IObservable<T> observable, bool condition, IScheduler scheduler, T toPrepend)
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

        private static IObservable<IReadOnlyList<NotifyCollectionChangedEventArgs>> Buffer(
            this IObservable<NotifyCollectionChangedEventArgs> observable,
            TimeSpan throttleTime,
            IScheduler scheduler,
            bool signalInitial)
        {
            observable = observable.StartWithIf(signalInitial, scheduler, CachedEventArgs.NotifyCollectionReset);

            if (throttleTime > TimeSpan.Zero)
            {
                var shared = observable.Publish()
                                       .RefCount();
                return shared.Buffer(() => shared.ThrottleOrDefault(throttleTime, scheduler))
                             .Cast<IReadOnlyList<NotifyCollectionChangedEventArgs>>();
            }

            if (scheduler == null)
            {
                return observable.Select(x => new[] { x });
            }

            return observable.Select(x => new[] { x })
                             .ObserveOn(scheduler);
        }
    }
}