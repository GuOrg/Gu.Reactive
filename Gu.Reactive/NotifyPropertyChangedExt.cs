namespace Gu.Reactive
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reflection;
    using System.Text;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Extension methods for subscribing to property changes.
    /// </summary>
    public static class NotifyPropertyChangedExt
    {
        private static readonly ConcurrentDictionary<LambdaExpression, VerifiedPropertyPath> Cached = new ConcurrentDictionary<LambdaExpression, VerifiedPropertyPath>(PropertyPathComparer.Default);

        /// <summary>
        /// Extension method for listening to property changes.
        /// Handles nested x => x.Level1.Level2.Level3
        /// Unsubscribes &amp; subscribes when each level changes.
        /// Handles nulls.
        /// </summary>
        /// <param name="source">The source instance to track changes for. </param>
        /// <param name="property">
        /// An expression specifying the property path to track.
        /// Example x => x.Foo.Bar.Meh
        ///  </param>
        /// <param name="signalInitial">
        /// If true OnNext is called immediately on subscribe
        /// </param>
        public static IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChanged<TNotifier, TProperty>(
            this TNotifier source,
            Expression<Func<TNotifier, TProperty>> property,
            bool signalInitial = true)
            where TNotifier : INotifyPropertyChanged
        {
            var verifiedPath = Cached.GetOrAdd(property, p => VerifiedPropertyPath.Create((Expression<Func<TNotifier, TProperty>>)p));
            if (!string.IsNullOrEmpty(verifiedPath.ErrorMessage))
            {
                throw new ArgumentException($"Error found in {property}" + Environment.NewLine + verifiedPath.ErrorMessage, nameof(property));
            }

            if (verifiedPath.Path.Count > 1)
            {
                return source.ObservePropertyChanged((PropertyPath<TNotifier, TProperty>)verifiedPath.Path, signalInitial);
            }

            return source.ObservePropertyChanged(verifiedPath.Path[0].PropertyInfo.Name, signalInitial);
        }

        /// <summary>
        /// Prefer other overloads with x => x.PropertyName for refactor friendliness.
        /// This is faster though.
        /// </summary>
        /// <param name="source"> The source instance to track changes for. </param>
        /// <param name="name"> The name of the property to track. Note that nested properties are not allowed. </param>
        /// <param name="signalInitial"> If true OnNext is called immediately on subscribe </param>
        public static IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChanged(
            this INotifyPropertyChanged source,
            string name,
            bool signalInitial = true)
        {
            var observable = source.ObservePropertyChanged()
                                   .Where(e => IsPropertyName(e.EventArgs, name));
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

            return observable;
        }

        /// <summary>
        /// This is a faster version of ObservePropertyChanged. It returns only the <see cref="PropertyChangedEventArgs"/> from source and not the EventPattern
        /// </summary>
        /// <param name="source"> The source instance to track changes for. </param>
        /// <param name="propertyName"> The name of the property to track. Note that nested properties are not allowed. </param>
        /// <param name="signalInitial"> If true OnNext is called immediately on subscribe </param>
        public static IObservable<PropertyChangedEventArgs> ObservePropertyChangedSlim(this INotifyPropertyChanged source, string propertyName, bool signalInitial = true)
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNullOrEmpty(propertyName, nameof(propertyName));
            if (source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) == null)
            {
                throw new ArgumentException($"The type {source.GetType()} does not have a property named {propertyName}", propertyName);
            }

            var observable = source.ObservePropertyChangedSlim()
                                   .Where(e => IsPropertyName(e, propertyName));
            if (signalInitial)
            {
                observable = observable.StartWith(new PropertyChangedEventArgs(propertyName));
            }

            return observable;
        }

        /// <summary>
        /// Observe propertychanges with values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="property">An expression specifying the property path.</param>
        /// <param name="signalInitial">If true OnNext is called immediately on subscribe.</param>
        /// <typeparam name="TNotifier">The type of <paramref name="source"/></typeparam>
        /// <typeparam name="TProperty">The type of the last property in the path.</typeparam>
        /// <returns>The <see cref="IObservable{T}"/> of type of type <see cref="EventPattern{TArgs}"/> of type <see cref="PropertyChangedAndValueEventArgs{TProperty}"/>.</returns>
        public static IObservable<EventPattern<PropertyChangedAndValueEventArgs<TProperty>>> ObservePropertyChangedWithValue<TNotifier, TProperty>(
            this TNotifier source,
            Expression<Func<TNotifier, TProperty>> property,
            bool signalInitial = true)
            where TNotifier : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            Ensure.NotNull(property, nameof(property));
            var verifiedPath = Cached.GetOrAdd(property, p => VerifiedPropertyPath.Create((Expression<Func<TNotifier, TProperty>>)p));
            if (!string.IsNullOrEmpty(verifiedPath.ErrorMessage))
            {
                throw new ArgumentException($"Error found in {property}" + Environment.NewLine + verifiedPath.ErrorMessage, nameof(property));
            }

            return source.ObservePropertyChangedWithValue((PropertyPath<TNotifier, TProperty>)verifiedPath.Path, signalInitial);
        }

        /// <summary>
        /// Observe property changes for <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        public static IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChanged(this INotifyPropertyChanged source)
        {
            Ensure.NotNull(source, nameof(source));
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
        /// Observe property changes for <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        public static IObservable<PropertyChangedEventArgs> ObservePropertyChangedSlim(this INotifyPropertyChanged source)
        {
            Ensure.NotNull(source, nameof(source));
            var observable = Observable.Create<PropertyChangedEventArgs>(
                o =>
                    {
                        PropertyChangedEventHandler handler = (_, e) => o.OnNext(e);
                        source.PropertyChanged += handler;
                        return Disposable.Create(() => source.PropertyChanged -= handler);
                    });
            return observable;
        }

        /// <summary>
        /// Extension method for listening to property changes.
        /// Handles nested x => x.Level1.Level2.Level3
        /// Unsubscribes &amp; subscribes when each level changes.
        /// Handles nulls.
        /// </summary>
        /// <param name="source">The source instance to track changes for. </param>
        /// <param name="propertyPath">
        /// A cached property path. Creating the property path from Expression&lt;Func&lt;TNotifier, TProperty&gt;&gt; is a bit expensive so caching can make sense.
        ///  </param>
        /// <param name="signalInitial">
        /// If true OnNext is called immediately on subscribe
        /// </param>
        internal static IObservable<EventPattern<PropertyChangedEventArgs>> ObservePropertyChanged<TNotifier, TProperty>(
            this TNotifier source,
            PropertyPath<TNotifier, TProperty> propertyPath,
            bool signalInitial = true)
            where TNotifier : INotifyPropertyChanged
        {
            if (signalInitial)
            {
                return Observable.Defer(
                    () => Observable.Return(
                                        new EventPattern<PropertyChangedEventArgs>(
                                            propertyPath.GetSender(source),
                                            new PropertyChangedEventArgs(propertyPath.Last.PropertyInfo.Name)))
                                    .Concat(source.ObservePropertyChanged(propertyPath, false)));
            }

            return Observable.Create<EventPattern<PropertyChangedEventArgs>>(
                o =>
                    {
                        var rootItem = new RootPropertyTracker(source);
                        var path = new PropertyPathTracker(rootItem, propertyPath);
                        var subscription = path[path.Count - 1]
                            .ObservePropertyChanged()
                            .Subscribe(o);
                        return new CompositeDisposable(3) { rootItem, path, subscription };
                    });
        }

        internal static IObservable<EventPattern<PropertyChangedAndValueEventArgs<TProperty>>> ObservePropertyChangedWithValue<TNotifier, TProperty>(
            this TNotifier source,
            PropertyPath<TNotifier, TProperty> propertyPath,
            bool signalInitial = true)
            where TNotifier : INotifyPropertyChanged
        {
            return source.ObservePropertyChanged(propertyPath, signalInitial)
                         .Select(
                             x => new EventPattern<PropertyChangedAndValueEventArgs<TProperty>>(
                                 x.Sender,
                                 new PropertyChangedAndValueEventArgs<TProperty>(
                                     x.EventArgs.PropertyName,
                                     propertyPath.Last.GetPropertyValue(x.Sender).As<TProperty>())));
        }

        private static bool IsPropertyName(this PropertyChangedEventArgs e, string propertyName)
        {
            return string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == propertyName;
        }

        private class VerifiedPropertyPath
        {
            private VerifiedPropertyPath(IPropertyPath path, string errorMessage)
            {
                this.Path = path;
                this.ErrorMessage = errorMessage;
            }

            internal IPropertyPath Path { get; }

            internal string ErrorMessage { get; }

            internal static VerifiedPropertyPath Create<TNotifier, TProperty>(Expression<Func<TNotifier, TProperty>> property)
            {
                var path = PropertyPath.GetOrCreate(property);
                var errorBuilder = new StringBuilder();
                for (var i = 0; i < path.Count - 1; i++)
                {
                    string errorMessage;
                    if (TryGetError(path, i, out errorMessage))
                    {
                        errorBuilder.Append(errorMessage);
                        errorBuilder.AppendLine();
                    }
                }

                return new VerifiedPropertyPath(path, errorBuilder.ToString());
            }

            private static bool TryGetError(IPropertyPath path, int i, out string errorMessage)
            {
                var propertyInfo = path[i].PropertyInfo;
                if (propertyInfo.PropertyType.IsValueType)
                {
                    errorMessage = string.Format(
                            "Property path cannot have structs in it. Copy by value will make subscribing error prone. Also mutable struct much?" + Environment.NewLine +
                            "The type {0} is a value type not so {1}.{2} will not notify when it changes." + Environment.NewLine +
                            "The path is: {3}",
                            propertyInfo.PropertyType.PrettyName(),
                            i == 0 ? "x" : path[i - 1].PropertyInfo.Name,
                            propertyInfo.Name,
                            path);
                    return true;
                }

                if (!typeof(INotifyPropertyChanged).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    errorMessage = string.Format(
                        "All levels in the path must implement INotifyPropertyChanged." + Environment.NewLine +
                        "The type {0} does not so {1}.{2} will not notify when it changes." + Environment.NewLine +
                        "The path is: {3}",
                        propertyInfo.PropertyType.PrettyName(),
                        i == 0 ? "x" : path[i - 1].PropertyInfo.Name,
                        propertyInfo.Name,
                        path);
                    return true;
                }

                errorMessage = null;
                return false;
            }
        }
    }
}
