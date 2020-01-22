namespace Gu.Reactive.Tests.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Linq;

    public static class TestExtensions
    {
        public static EventList SubscribeAll<T>(this T observableCollection)
            where T : IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
        {
            return EventList.Create(observableCollection);
        }

        public static IReadOnlyList<NotifyCollectionChangedEventArgs> CollectionChanges(this INotifyCollectionChanged col)
        {
            var changes = new List<NotifyCollectionChangedEventArgs>();
            col.CollectionChanged += (_, e) => changes.Add(e);
            return changes;
        }

        public static IMaybe<NotifyCollectionChangedEventArgs> CollectionChange(this INotifyCollectionChanged col)
        {
            var maybe = new Maybe<NotifyCollectionChangedEventArgs>();
            col.CollectionChanged += (_, e) =>
                {
                    if (maybe.HasValue)
                    {
                        throw new InvalidOperationException("Expected only one notification");
                    }

                    maybe.Value = e;
                };
            return maybe;
        }

        public sealed class EventList : IReadOnlyCollection<EventArgs>, IDisposable
        {
            private readonly List<EventArgs> inner = new List<EventArgs>();
            private readonly IDisposable subscription;

            private bool disposed;

            private EventList(
                IObservable<NotifyCollectionChangedEventArgs> incc,
                IObservable<PropertyChangedEventArgs> inpc)
            {
                this.subscription = Observable.Merge<EventArgs>(incc, inpc)
                                              .Subscribe(this.inner.Add);
            }

            public int Count => this.inner.Count;

            public static EventList Create<T>(T observableCollection)
                                where T : IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
            {
                return new EventList(
                    observableCollection.ObserveCollectionChangedSlim(signalInitial: false),
                    observableCollection.ObservePropertyChangedSlim());
            }

            public IEnumerator<EventArgs> GetEnumerator()
            {
                return this.inner.GetEnumerator();
            }

            public void Dispose()
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
                this.subscription?.Dispose();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)this.inner).GetEnumerator();
            }
        }

        private class Maybe<TNotifyCollectionChangedEventArgs> : IMaybe<TNotifyCollectionChangedEventArgs>
        {
            public bool HasValue => this.Value != null;

            public TNotifyCollectionChangedEventArgs Value { get; set; } = default!;
        }
    }
}
