namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Linq;
    using Internals;

    /// <summary>
    /// The property changed to observable ext.
    /// </summary>
    public static class NotifyPropertyChangedExt
    {
        /// <summary>
        /// CConvenience wrapper for listening to property changes
        /// </summary>
        /// <typeparam name="TNotifier">
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// </typeparam>
        /// <param name="source">
        /// </param>
        /// <param name="property">
        /// </param>
        /// <param name="signalInitial">
        /// If true OnNext is called immediately on subscribe
        /// </param>
        /// <returns>
        /// The <see cref="IObservable"/>.
        /// </returns>
        public static IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChanged<TNotifier, TProperty>(
            this TNotifier source,
            Expression<Func<TNotifier, TProperty>> property,
            bool signalInitial = true) where TNotifier : INotifyPropertyChanged
        {
            var me = (MemberExpression)property.Body;
            var pe = me.Expression as ParameterExpression;
            if (pe == null)
            {
                var observable = new PropertyPathObservable<TNotifier, TProperty>(source, property);
                if (signalInitial)
                {
                    return Observable.Defer(
                        () =>
                        {
                            var current = new EventPattern<PropertyChangedEventArgs>(
                                observable.Sender,
                                observable.PropertyChangedEventArgs);
                            return Observable.Return(current)
                                             .Concat(observable);
                        });
                }

                return observable;
            }

            string name = me.Member.Name;
            return source.ObservePropertyChanged(name, signalInitial);
        }

        /// <summary>
        /// The to observable.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="signalInitial">
        /// If true OnNext is called immediately on subscribe
        /// </param>
        /// <returns>
        /// The <see cref="IObservable"/>.
        /// </returns>
        public static IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChanged(
            this INotifyPropertyChanged source,
            string name,
            bool signalInitial = true)
        {
            var observable = source.ObservePropertyChanged()
                                   .Where(e => IsPropertyName(e, name));
            if (signalInitial)
            {
                var wr = new WeakReference(source);
                return Observable.Defer(
                    () =>
                    {
                        var current = new EventPattern<PropertyChangedEventArgs>(wr.Target, new PropertyChangedEventArgs(name));
                        return Observable.Return(current)
                                         .Concat(observable);
                    });
            }
            else
            {
                return observable;
            }
        }

        /// <summary>
        /// The to tracking observable.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="signalInitial">
        /// If true OnNext is called immediately on subscribe
        /// </param>
        /// <typeparam name="TNotifier">
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IObservable"/>.
        /// </returns>
        public static IObservable<EventPattern<PropertyChangedAndValueEventArgs<TProperty>>> ObservePropertyChangedWithValue
            <TNotifier, TProperty>(
            this TNotifier source,
            Expression<Func<TNotifier, TProperty>> property,
            bool signalInitial = true)
            where TNotifier : INotifyPropertyChanged
        {
            var wr = new WeakReference(source);
            var name = NameOf.Property(property);
            var observable = source.ObservePropertyChanged(property, false);

            return Observable.Defer(
                () =>
                {
                    var valuePath = PropertyPath.Create(property);

                    var withValues = observable.Select(x => new EventPattern<PropertyChangedAndValueEventArgs<TProperty>>(
                                                            x.Sender,
                                                            new PropertyChangedAndValueEventArgs<TProperty>(
                                                                x.EventArgs.PropertyName,
                                                                valuePath.GetValue((TNotifier)wr.Target))));
                    if (signalInitial)
                    {
                        var valueAndSource = valuePath.GetValueAndSender((TNotifier)wr.Target);
                        var current = new EventPattern<PropertyChangedAndValueEventArgs<TProperty>>(
                            valueAndSource.Source,
                            new PropertyChangedAndValueEventArgs<TProperty>(
                                name,
                                valueAndSource.Value));
                        return Observable.Return(current)
                                         .Concat(withValues);
                    }

                    return withValues;
                });
        }

        /// <summary>
        /// The to observable.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <returns>
        /// The <see cref="IObservable"/>.
        /// </returns>
        public static IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChanged(
            this INotifyPropertyChanged source)
        {
            var wr = new WeakReference<INotifyPropertyChanged>(source);
            IObservable<EventPattern<PropertyChangedEventArgs>> observable =
                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    x =>
                    {
                        INotifyPropertyChanged inpc;
                        if (wr.TryGetTarget(out inpc))
                        {
                            inpc.PropertyChanged += x;
                        }
                    },
                    x =>
                    {
                        INotifyPropertyChanged inpc;
                        if (wr.TryGetTarget(out inpc))
                        {
                            inpc.PropertyChanged -= x;
                        }
                    });
            return observable;
        }

        private static bool IsPropertyName(EventPattern<PropertyChangedEventArgs> e, string propertyName)
        {
            return string.IsNullOrEmpty(e.EventArgs.PropertyName) || e.EventArgs.PropertyName == propertyName;
        }
    }
}
