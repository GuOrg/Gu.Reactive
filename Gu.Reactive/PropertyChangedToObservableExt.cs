namespace Gu.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Linq;

    public static class PropertyChangedToObservableExt
    {
        public static IObservable<EventPattern<PropertyChangedEventArgs>> ToObservable<TNotifier, TProperty>(
            this TNotifier source,
            Expression<Func<TNotifier, TProperty>> property,
            bool sampleCurrent = true)
            where TNotifier : INotifyPropertyChanged
        {
            var me = (MemberExpression)property.Body;
            var pe = me.Expression as ParameterExpression;
            if (pe == null)
            {
                return new NestedObservable<TNotifier, TProperty>(source, property);
            }
            string name = me.Member.Name;
            return source.ToObservable(name, sampleCurrent);
        }

        public static IObservable<EventPattern<PropertyChangedEventArgs>> ToObservable(
            this INotifyPropertyChanged source, string name, bool sampleCurrent = true)
        {
            var wr = new WeakReference(source);
            var observable = source.ToObservable()
                                   .Where(e => string.IsNullOrEmpty(e.EventArgs.PropertyName) || e.EventArgs.PropertyName == name);
            if (sampleCurrent)
            {
                return Observable.Defer(
                    () =>
                    {
                        var current = new EventPattern<PropertyChangedEventArgs>(wr.Target, new PropertyChangedEventArgs(name));
                        return Observable.Return(current).Concat(observable);
                    });
            }
            else
            {
                return observable;
            }
        }

        public static IObservable<PropertyChangedTrackingEventArgs<TNotifier, TProperty>> ToTrackingObservable<TNotifier, TProperty>(
            this TNotifier source,
            Expression<Func<TNotifier, TProperty>> property,
            bool sampleCurrent)
            where TNotifier : INotifyPropertyChanged
        {
            var wr = new WeakReference(source);
            Func<TNotifier, TProperty> getter = property.Compile();
            var observable = source.ToObservable(property, sampleCurrent)
                                   .Scan(
                                       new PropertyChangedTrackingEventArgs<TNotifier, TProperty>(
                                           (TNotifier)wr.Target,
                                           default(TProperty),
                                           default(TProperty),
                                           ""),
                                       (acc, cur) => new PropertyChangedTrackingEventArgs<TNotifier, TProperty>(
                                           (TNotifier)wr.Target,
                                           GetOrDefault(wr, getter),
                                           acc.CurrentValue,
                                           acc.PropertyName));
            return observable;
        }

        public static IObservable<EventPattern<PropertyChangedEventArgs>> ToObservable(this INotifyPropertyChanged source)
        {
            var wr = new WeakReference<INotifyPropertyChanged>(source);
            IObservable<EventPattern<PropertyChangedEventArgs>> observable = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
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
