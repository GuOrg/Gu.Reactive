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

    /// <summary>
    /// The nested observable.
    /// </summary>
    /// <typeparam name="TClass">
    /// </typeparam>
    /// <typeparam name="TProp">
    /// </typeparam>
    internal sealed class PathObservable<TClass, TProp> : ObservableBase<EventPattern<PropertyChangedEventArgs>>, IDisposable
        where TClass : INotifyPropertyChanged
    {
        /// <summary>
        /// The _value.
        /// </summary>
        private readonly WeakReference _value;

        /// <summary>
        /// The _subject.
        /// </summary>
        private readonly Subject<EventPattern<PropertyChangedEventArgs>> _subject = new Subject<EventPattern<PropertyChangedEventArgs>>();

        internal readonly IReadOnlyList<PathItem> ValuePath;
        private bool _disposed;

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
            this.ValuePath = Reactive.ValuePath.Create(propertyExpression);
            this.ValuePath[0].Source = source;
            AssertPathNotifies(this.ValuePath);
            AddSubscriptions(0);
            _value = new WeakReference(this.ValuePath.Last().Value, false);
        }

        public void Dispose()
        {
            foreach (var pathItem in this.ValuePath)
            {
                pathItem.Dispose();
            }
            _subject.Dispose();
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
            VerifyDisposed();
            return _subject.Subscribe(observer);
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
            if (toIndex > 0 && toIndex < this.ValuePath.Count)
            {
                this.ValuePath[toIndex].Source = (INotifyPropertyChanged)this.ValuePath[toIndex - 1].Value;
            }

            for (int j = toIndex; j < this.ValuePath.Count; j++)
            {
                var pathItem = this.ValuePath[j];
                var o = pathItem.Source;

                if (o == null)
                {
                    break;
                }

                pathItem.Subscription = o.ObservePropertyChanged(pathItem.PropertyInfo.Name, false)
                                         .Subscribe(OnPathItemChanged);
                if (!pathItem.IsLast)
                {
                    this.ValuePath[j + 1].Source = (INotifyPropertyChanged)pathItem.Value;
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
            for (int j = fromIndex; j < this.ValuePath.Count; j++)
            {
                var pathItem = this.ValuePath[j];
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

            var value = this.ValuePath.Last().Value;
            if (!(value == null && _value.Target == null))
            {
                _value.Target = value;
                var pattern = new EventPattern<PropertyChangedEventArgs>(this.ValuePath.Last().Source, new PropertyChangedEventArgs(this.ValuePath.Last().PropertyInfo.Name));
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
            for (int i = 0; i < this.ValuePath.Count; i++)
            {
                var pathItem = this.ValuePath[i];
                if (ReferenceEquals(pathItem.Source, sender))
                {
                    return i;
                }
            }

            throw new ArgumentOutOfRangeException();
        }

        private void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

    }
}