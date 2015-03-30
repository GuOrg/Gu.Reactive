// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangedToObservableExt.cs" company="">
//   
// </copyright>
// <summary>
//   The property changed to observable ext.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Linq;

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
        /// Default true means that the current value is signaled on Subscribe()
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
                var wr = new WeakReference(source);
                var observable = new PathObservable<TNotifier, TProperty>(source, property);
                if (signalInitial)
                {
                    return Observable.Defer(
                        () =>
                        {
                            var current = new EventPattern<PropertyChangedEventArgs>(
                                wr.Target,
                                new PropertyChangedEventArgs(
                                    observable.Path.Last()
                                              .PropertyInfo.Name));
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
        /// The signal initial.
        /// </param>
        /// <returns>
        /// The <see cref="IObservable"/>.
        /// </returns>
        public static IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChanged(
            this INotifyPropertyChanged source,
            string name,
            bool signalInitial = true)
        {
            var wr = new WeakReference(source);
            var observable = source.ObservePropertyChanged()
                                   .Where(
                                       e =>
                                       string.IsNullOrEmpty(e.EventArgs.PropertyName)
                                       || e.EventArgs.PropertyName == name);
            if (signalInitial)
            {
                return Observable.Defer(
                    () =>
                    {
                        var current = new EventPattern<PropertyChangedEventArgs>(
                            wr.Target,
                            new PropertyChangedEventArgs(name));
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
        /// <param name="sampleCurrent">
        /// The sample current.
        /// </param>
        /// <typeparam name="TNotifier">
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IObservable"/>.
        /// </returns>
        public static IObservable<TrackingEventArgs<TNotifier, TProperty>> ToTrackingObservable
            <TNotifier, TProperty>(
            this TNotifier source,
            Expression<Func<TNotifier, TProperty>> property,
            bool sampleCurrent) where TNotifier : INotifyPropertyChanged
        {
            var wr = new WeakReference(source);
            Func<TNotifier, TProperty> getter = property.Compile();
            var observable = source.ObservePropertyChanged(property, sampleCurrent)
                                   .Scan(
                                       new TrackingEventArgs<TNotifier, TProperty>(
                                           (TNotifier)wr.Target,
                                           default(TProperty),
                                           default(TProperty),
                                           string.Empty),
                                       (acc, cur) =>
                                       new TrackingEventArgs<TNotifier, TProperty>(
                                           (TNotifier)wr.Target,
                                           GetOrDefault(wr, getter),
                                           acc.CurrentValue,
                                           acc.PropertyName));
            return observable;
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

        /// <summary>
        /// The get or default.
        /// </summary>
        /// <param name="wr">
        /// The wr.
        /// </param>
        /// <param name="getter">
        /// The getter.
        /// </param>
        /// <typeparam name="TSource">
        /// </typeparam>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        private static T GetOrDefault<TSource, T>(WeakReference wr, Func<TSource, T> getter)
        {
            var target = wr.Target;
            if (target == null)
            {
                return default(T);
            }

            return getter((TSource)target);
        }

        ////    public static IObservable<TProperty> ToObservable<TNotifier, TProperty>(this OcNpcListener<TNotifier> source,
        ////Expression<Func<TNotifier, TProperty>> property) where TNotifier : INotifyPropertyChanged
        ////    {
        ////        string name = ((MemberExpression)property.Body).Member.Name;
        ////        Func<TNotifier, TProperty> getter = property.Compile();
        ////        IObservable<TProperty> observable = source.ToObservable()
        ////            .Select(x => x.EventArgs)
        ////            .Where(x => x.PropertyName == name)
        ////            .Select(x => getter((TNotifier)x.Child));
        ////        return observable;
        ////    }
        ////    public static IObservable<EventPattern<ChildPropertyChangedEventArgs>> ToObservable<T>(this OcNpcListener<T> source) where T : INotifyPropertyChanged
        ////    {
        ////        IObservable<EventPattern<ChildPropertyChangedEventArgs>> observable = Observable
        ////            .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
        ////                x => source.PropertyChanged += x,
        ////                x => source.PropertyChanged -= x)
        ////                .Select(x => new EventPattern<ChildPropertyChangedEventArgs>(x.Sender, (ChildPropertyChangedEventArgs)x.EventArgs));
        ////        return observable;
        ////    }
        ////    }
    }
}
