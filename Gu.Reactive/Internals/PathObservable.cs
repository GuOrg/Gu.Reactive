namespace Gu.Reactive.Internals
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
        internal readonly IReadOnlyList<NotifyingPathItem> ValuePath;
        /// <summary>
        /// The _subject.
        /// </summary>
        private readonly Subject<EventPattern<PropertyChangedEventArgs>> _subject = new Subject<EventPattern<PropertyChangedEventArgs>>();
        private readonly IDisposable _subscription;

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
            ValuePath = Internals.ValuePath.CreateNotifyingPropertyPath(propertyExpression);
            ValuePath[0].Source = source;
            AssertPathNotifies(ValuePath);
            _subscription = ValuePath.Last().ObservePropertyChanged().Subscribe(x => _subject.OnNext(x));
        }

        public void Dispose()
        {
            foreach (var pathItem in ValuePath)
            {
                pathItem.Dispose();
            }
            _subscription.Dispose();
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

        private void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}