namespace Gu.Reactive
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;

    public static class NotifyCollectionChangedExt
    {
        public static IObservable<EventPattern<NotifyCollectionChangedEventArgs>> ObserveCollectionChanged(
            this INotifyCollectionChanged source,
            bool signalInitial = true)
        {
            IObservable<EventPattern<NotifyCollectionChangedEventArgs>> observable =
                Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    x => source.CollectionChanged += x,
                    x => source.CollectionChanged -= x);
            if (signalInitial)
            {
                var wr = new WeakReference(source);
                return Observable.Defer(
                    () =>
                        {
                            var current = new EventPattern<NotifyCollectionChangedEventArgs>(
                                wr.Target,
                                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                            return Observable.Return(current)
                                             .Concat(observable);
                        });
            }

            return observable;
        }

        internal static IObservable<EventPattern<ChildPropertyChangedEventArgs<T, TProperty>>> ObserveItemPropertyChanges<T, TProperty>(
            this ObservableCollection<T> source,
            Expression<Func<T, TProperty>> property,
            bool signalInitial = true) where T : class, INotifyPropertyChanged
        {
            return new ItemsObservable<T, TProperty>(source, property, signalInitial);
        }
    }
}