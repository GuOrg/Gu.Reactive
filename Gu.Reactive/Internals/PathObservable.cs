namespace Gu.Reactive.Internals
{
    using System;
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
    internal sealed class PathObservable<TClass, TProp> :
        ObservableBase<EventPattern<PropertyChangedEventArgs>>,
        IDisposable
        where TClass : INotifyPropertyChanged
    {
        internal readonly PropertyChangedEventArgs PropertyChangedEventArgs;

        internal readonly NotifyingPath _valuePath;

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
            _valuePath = NotifyingPath.Create(propertyExpression);
            _valuePath.Source = source;
            var last = _valuePath.Last();
            PropertyChangedEventArgs = last.PropertyChangedEventArgs;
            VerifyPath(_valuePath);
            _subscription = last.ObservePropertyChanged()
                                .Subscribe(OnNext, OnError);
        }

        public void Dispose()
        {
            foreach (var pathItem in _valuePath)
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
        private static void VerifyPath(NotifyingPath path)
        {
            for (int i = 1; i < path.Count; i++)
            {
                var propertyInfo = path[i].PathItem.PropertyInfo;
                if ((i != path.Count - 1) && propertyInfo.PropertyType.IsValueType)
                {
                    throw new ArgumentException(
                        "Property path cannot have structs in it. Copy by value will make subscribing error prone. Also mutable struct much?");
                }
                if (!ImplementsINotifyPropertyChanged(propertyInfo.DeclaringType))
                {
                    throw new ArgumentException(
                        string.Format(
                            "All levels in the path must notify (DeclaringType is INotifyPropertyChanged) the type {0} does not so {0}.{1} will not notify.",
                            propertyInfo.DeclaringType.Name,
                            propertyInfo.Name));
                }
            }
        }

        private static bool ImplementsINotifyPropertyChanged(Type type)
        {
            return type.GetInterfaces()
                       .Contains(typeof(INotifyPropertyChanged));
        }

        private void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(
                    GetType()
                        .FullName);
            }
        }

        private void OnNext(EventPattern<PropertyChangedEventArgs> e)
        {
            _subject.OnNext(e);
        }

        private void OnError(Exception e)
        {
            _subject.OnError(e);
        }
    }
}