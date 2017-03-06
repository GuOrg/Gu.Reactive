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
                                   .Where(e => IsMatch(e.EventArgs, name));
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
        public static IObservable<PropertyChangedEventArgs> ObservePropertyChangedSlim<TNotifier, TProperty>(
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
                return source.ObservePropertyChangedSlim((PropertyPath<TNotifier, TProperty>)verifiedPath.Path, signalInitial);
            }

            return source.ObservePropertyChangedSlim(verifiedPath.Path[0].PropertyInfo.Name, signalInitial);
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
                                   .Where(e => IsMatch(e, propertyName));
            if (signalInitial)
            {
                observable = observable.StartWith(new PropertyChangedEventArgs(propertyName));
            }

            return observable;
        }

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
        public static IObservable<Maybe<TProperty>> ObserveValue<TNotifier, TProperty>(
            this TNotifier source,
            Expression<Func<TNotifier, TProperty>> property,
            bool signalInitial = true)
            where TNotifier : class, INotifyPropertyChanged
        {
            Ensure.NotNull(source, nameof(source));
            var verifiedPath = Cached.GetOrAdd(property, p => VerifiedPropertyPath.Create((Expression<Func<TNotifier, TProperty>>)p));
            if (!string.IsNullOrEmpty(verifiedPath.ErrorMessage))
            {
                throw new ArgumentException($"Error found in {property}" + Environment.NewLine + verifiedPath.ErrorMessage, nameof(property));
            }

            return source.ObserveValue((PropertyPath<TNotifier, TProperty>)verifiedPath.Path, signalInitial);
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
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                x => source.PropertyChanged += x,
                x => source.PropertyChanged -= x);
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
                return Observable.Return(new EventPattern<PropertyChangedEventArgs>(
                                         propertyPath.GetSender(source),
                                         CachedEventArgs.GetOrCreatePropertyChangedEventArgs(string.Empty)))
                                 .Concat(source.ObservePropertyChanged(propertyPath, false));
            }

            return Observable.Create<EventPattern<PropertyChangedEventArgs>>(
                o =>
                    {
                        var path = new PropertyPathTracker(source, propertyPath);
                        PropertyChangedEventHandler handler = (sender, e) => o.OnNext(new EventPattern<PropertyChangedEventArgs>(sender, e));
                        path.Last.TrackedPropertyChanged += handler;
                        return new CompositeDisposable(2)
                        {
                            path,
                            Disposable.Create(() => path.Last.TrackedPropertyChanged -= handler)
                        };
                    });
        }

        internal static IObservable<PropertyChangedEventArgs> ObservePropertyChangedSlim<TNotifier, TProperty>(
            this TNotifier source,
            PropertyPath<TNotifier, TProperty> propertyPath,
            bool signalInitial = true)
            where TNotifier : INotifyPropertyChanged
        {
            if (signalInitial)
            {
                return Observable.Return(CachedEventArgs.GetOrCreatePropertyChangedEventArgs(string.Empty))
                                    .Concat(source.ObservePropertyChangedSlim(propertyPath, false));
            }

            return Observable.Create<PropertyChangedEventArgs>(
                o =>
                {
                    var path = new PropertyPathTracker(source, propertyPath);
                    PropertyChangedEventHandler handler = (_, e) => o.OnNext(e);
                    path.Last.TrackedPropertyChanged += handler;
                    return new CompositeDisposable(2)
                    {
                        path,
                        Disposable.Create(() => path.Last.TrackedPropertyChanged -= handler)
                    };
                });
        }

        internal static IObservable<Maybe<TProperty>> ObserveValue<TNotifier, TProperty>(
            this TNotifier source,
            PropertyPath<TNotifier, TProperty> propertyPath,
            bool signalInitial = true)
            where TNotifier : class, INotifyPropertyChanged
        {
            if (signalInitial)
            {
                return Observable.Defer(() => Observable.Return(propertyPath.Last.GetValueFromRoot<TProperty>(source))
                                                        .Concat(source.ObserveValue(propertyPath, false)));
            }

            if (propertyPath.Count > 1)
            {
                return Observable.Create<Maybe<TProperty>>(
                    o =>
                    {
                        var path = new PropertyPathTracker(source, propertyPath);
                        PropertyChangedEventHandler handler = (sender, _) =>
                        {
                            o.OnNext(propertyPath.Last.GetPropertyValue(sender)
                                                 .Cast<TProperty>());
                        };
                        path.Last.TrackedPropertyChanged += handler;
                        return new CompositeDisposable(2)
                        {
                            path,
                            Disposable.Create(() => path.Last.TrackedPropertyChanged -= handler)
                        };
                    });
            }

            return Observable.Create<Maybe<TProperty>>(
                o =>
                {
                    PropertyChangedEventHandler handler = (sender, _) =>
                    {
                        if (_.IsMatch(propertyPath.Last.PropertyInfo))
                        {
                            o.OnNext(propertyPath.Last.GetPropertyValue(sender)
                                                 .Cast<TProperty>());
                        }
                    };
                    source.PropertyChanged += handler;
                    return Disposable.Create(() => source.PropertyChanged -= handler);
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
                                     propertyPath.Last.GetPropertyValue(x.Sender).Cast<TProperty>())));
        }

        internal static bool IsMatch(this PropertyChangedEventArgs e, PropertyInfo property)
        {
            return IsMatch(e, property.Name);
        }

        internal static bool IsMatch(this PropertyChangedEventArgs e, string propertyName)
        {
            return string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == propertyName;
        }

        private class VerifiedPropertyPath
        {
            private readonly IPropertyPath path;

            private VerifiedPropertyPath(IPropertyPath path, string errorMessage)
            {
                this.path = path;
                this.ErrorMessage = errorMessage;
            }

            internal string ErrorMessage { get; }

            internal IPropertyPath Path
            {
                get
                {
                    if (!string.IsNullOrEmpty(this.ErrorMessage))
                    {
                        throw new InvalidOperationException($"Error found in {this.path}" + Environment.NewLine + this.ErrorMessage);
                    }

                    return this.path;
                }
            }

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
