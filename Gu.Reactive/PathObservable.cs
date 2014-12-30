// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NestedObservable.cs" company="">
//   
// </copyright>
// <summary>
//   The nested observable.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Subjects;
    using System.Reflection;

    /// <summary>
    /// The nested observable.
    /// </summary>
    /// <typeparam name="TClass">
    /// </typeparam>
    /// <typeparam name="TProp">
    /// </typeparam>
    internal class PathObservable<TClass, TProp> : ObservableBase<EventPattern<PropertyChangedEventArgs>>, IDisposable
        where TClass : INotifyPropertyChanged
    {
        /// <summary>
        /// The _value.
        /// </summary>
        private readonly WeakReference _value;

        /// <summary>
        /// The _path.
        /// </summary>
        private readonly List<PathItem> _path = new List<PathItem>();

        /// <summary>
        /// The _subject.
        /// </summary>
        private readonly Subject<EventPattern<PropertyChangedEventArgs>> _subject = new Subject<EventPattern<PropertyChangedEventArgs>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PathObservable{TClass,TProp}"/> class.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="propertyExpression">
        /// The property expression.
        /// </param>
        public PathObservable(TClass source, Expression<Func<TClass, TProp>> propertyExpression)
        {
            var path = PathExpressionVisitor.GetPath(propertyExpression);
            foreach (var pathItem in path)
            {
                var item = new PathItem(pathItem.Expression.Type.GetProperty(pathItem.Member.Name));
                _path.Add(item);
            }

            _path.First().Source = source;
            _path.Last().IsLast = true;
            AssertPathNotifies(_path);
            AddSubscriptions(0);
            _value = new WeakReference(_path.Last().Value, false);
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        internal IEnumerable<PathItem> Path
        {
            get
            {
                return _path;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The subscribe core.
        /// </summary>
        /// <param name="observer">
        /// The observer.
        /// </param>
        /// <returns>
        /// The <see cref="IDisposable"/>.
        /// </returns>
        protected override IDisposable SubscribeCore(IObserver<EventPattern<PropertyChangedEventArgs>> observer)
        {
            return _subject.Subscribe(observer);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_path.Any())
                {
                    foreach (var pathItem in _path)
                    {
                        pathItem.Dispose();
                    }

                    _path.Clear();
                }
            }
        }

        /// <summary>
        /// All steps in the path must implement INotifyPropertyChanged, this throws if this condition is not met.
        /// </summary>
        /// <param name="path">
        /// </param>
        private static void AssertPathNotifies(IEnumerable<PathItem> path)
        {
            var notNotifyings = path.Where(x => !x.PropertyInfo.DeclaringType.GetInterfaces()
                                               .Contains(typeof(INotifyPropertyChanged)))
                                    .ToArray();
            if (notNotifyings.Any())
            {
                var props = string.Join(", ", notNotifyings.Select(x => "." + x.PropertyInfo.Name));
                throw new ArgumentException(string.Format("All levels in the path must notify (parent must be : INotifyPropertyChanged)  {{{0}}} does not.", props));
            }
        }

        /// <summary>
        /// The add subscriptions.
        /// </summary>
        /// <param name="toIndex">
        /// The to index.
        /// </param>
        private void AddSubscriptions(int toIndex)
        {
            if (toIndex > 0 && toIndex < _path.Count)
            {
                _path[toIndex].Source = _path[toIndex - 1].Value;
            }

            for (int j = toIndex; j < _path.Count; j++)
            {
                var pathItem = _path[j];
                var o = pathItem.Source;

                if (o == null)
                {
                    break;
                }

                pathItem.Subscription = o.ToObservable(pathItem.PropertyInfo.Name, false)
                                         .Subscribe(OnPathItemChanged);
                if (!pathItem.IsLast)
                {
                    _path[j + 1].Source = pathItem.Value;
                }
            }
        }

        /// <summary>
        /// The remove subscriptions.
        /// </summary>
        /// <param name="fromIndex">
        /// The from index.
        /// </param>
        private void RemoveSubscriptions(int fromIndex)
        {
            for (int j = fromIndex; j < _path.Count; j++)
            {
                var pathItem = _path[j];
                if (pathItem.Subscription == null)
                {
                    break;
                }

                pathItem.Subscription.Dispose();
                pathItem.Subscription = null;
                pathItem.Source = null;
            }
        }

        /// <summary>
        /// The on path item changed.
        /// </summary>
        /// <param name="eventPattern">
        /// The event pattern.
        /// </param>
        private void OnPathItemChanged(EventPattern<PropertyChangedEventArgs> eventPattern)
        {
            var i = IndexOf((INotifyPropertyChanged)eventPattern.Sender);
            RemoveSubscriptions(i + 1);
            AddSubscriptions(i + 1);

            var value = _path.Last().Value;
            if (!(value == null && _value.Target == null))
            {
                _value.Target = value;
                var pattern = new EventPattern<PropertyChangedEventArgs>(_path.Last().Source, new PropertyChangedEventArgs(_path.Last().PropertyInfo.Name));
                _subject.OnNext(pattern);
            }
        }

        /// <summary>
        /// The index of.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        private int IndexOf(INotifyPropertyChanged sender)
        {
            for (int i = 0; i < _path.Count; i++)
            {
                var pathItem = _path[i];
                if (ReferenceEquals(pathItem.Source, sender))
                {
                    return i;
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// The path item.
        /// </summary>
        internal class PathItem : IDisposable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PathItem"/> class.
            /// </summary>
            /// <param name="propertyInfo">
            /// The property info.
            /// </param>
            public PathItem(PropertyInfo propertyInfo)
            {
                PropertyInfo = propertyInfo;
            }

            /// <summary>
            /// Gets or sets the source.
            /// </summary>
            public INotifyPropertyChanged Source { get; set; }

            /// <summary>
            /// Gets the property info.
            /// </summary>
            public PropertyInfo PropertyInfo { get; private set; }

            /// <summary>
            /// Gets or sets the subscription.
            /// </summary>
            public IDisposable Subscription { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether is last.
            /// </summary>
            public bool IsLast { get; set; }

            /// <summary>
            /// Gets the value.
            /// </summary>
            public dynamic Value
            {
                get
                {
                    if (Source == null)
                    {
                        return null;
                    }

                    return PropertyInfo.GetMethod.Invoke(Source, null);
                }
            }

            /// <summary>
            /// The to string.
            /// </summary>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public override string ToString()
            {
                return string.Format("{0}.{1}", Source != null ? Source.GetType().Name : "null", PropertyInfo.Name);
            }

            /// <summary>
            /// The dispose.
            /// </summary>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// The dispose.
            /// </summary>
            /// <param name="disposing">
            /// The disposing.
            /// </param>
            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (Subscription != null)
                    {
                        Subscription.Dispose();
                        Subscription = null;
                    }
                }
            }
        }
    }
}