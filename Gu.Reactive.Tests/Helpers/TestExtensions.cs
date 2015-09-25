namespace Gu.Reactive.Tests.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public static class TestExtensions
    {
        public static List<EventArgs> SubscribeAll<T>(this T observableCollection)
            where T : IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
        {
            var changes = new List<EventArgs>();
            observableCollection.ObserveCollectionChangedSlim(false)
                                .Subscribe(x => changes.Add(x));
            observableCollection.ObservePropertyChanged()
                                .Subscribe(x => changes.Add(x.EventArgs));
            return changes;
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

        private class Maybe<NotifyCollectionChangedEventArgs> : IMaybe<NotifyCollectionChangedEventArgs>
        {
            public bool HasValue => Value != null;
            public NotifyCollectionChangedEventArgs Value { get; set; }
        }
    }
}
