namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.ComponentModel;
    using System.Reactive;
    using System.Reactive.Disposables;

    using Gu.Reactive.Internals;

    internal sealed class NotifyingPathItem : INotifyingPathItem
    {
        private readonly WeakReference _sourceRef = new WeakReference(null);
        private readonly Action<EventPattern<PropertyChangedEventArgs>> _onNext;
        private readonly Action<Exception> _onError;
        private bool _disposed;
        private readonly SerialDisposable _subscription = new SerialDisposable();

        public NotifyingPathItem(INotifyingPathItem previous, PathProperty pathProperty)
        {
            Ensure.NotNull(pathProperty, nameof(pathProperty));
            if (previous?.PathProperty != null)
            {
                var type = previous.PathProperty.PropertyInfo.PropertyType;
                if (type.IsValueType)
                {
                    var message = string.Format(
                            "Property path cannot have structs in it. Copy by value will make subscribing error prone." + Environment.NewLine +
                        "The type {0} is a value type not so {1}.{2} will not notify when it changes.",
                        type.PrettyName(),
                        previous.PathProperty.PropertyInfo,
                        pathProperty.PropertyInfo.Name);
                    throw new ArgumentException(message, nameof(pathProperty));
                }

                if (!typeof(INotifyPropertyChanged).IsAssignableFrom(type))
                {
                    var message = string.Format(
                        "All levels in the path must implement INotifyPropertyChanged." + Environment.NewLine +
                        "The type {0} does not so {1}.{2} will not notify when it changes.",
                        type.PrettyName(),
                        previous.PathProperty.PropertyInfo,
                        pathProperty.PropertyInfo.Name);
                    throw new ArgumentException(message, nameof(pathProperty));
                }
            }

            PathProperty = pathProperty;
            _onNext = x => OnPropertyChanged(x.Sender, x.EventArgs);
            _onError = OnError;
            PropertyChangedEventArgs = new PropertyChangedEventArgs(PathProperty.PropertyInfo.Name);
            Previous = previous;
            var notifyingPathItem = previous as NotifyingPathItem;
            if (notifyingPathItem != null)
            {
                notifyingPathItem.Next = this;
            }

            if (previous != null)
            {
                Source = (INotifyPropertyChanged)previous.Value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PathProperty PathProperty { get; }

        public INotifyingPathItem Previous { get; private set; }

        public NotifyingPathItem Next { get; private set; }

        public PropertyChangedEventArgs PropertyChangedEventArgs { get; }

        public bool IsLast => PathProperty.IsLast;

        public object Value
        {
            get
            {
                var source = Source;
                if (source == null)
                {
                    return null;
                }

                return PathProperty.PropertyInfo.GetValue(source);
            }
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public INotifyPropertyChanged Source
        {
            get
            {
                return (INotifyPropertyChanged)_sourceRef.Target;
            }
            set
            {
                var oldSource = _sourceRef.Target;
                _sourceRef.Target = value;
                var inpc = value;

                var isNullToNull = IsNullToNull(oldSource, value);
                if (inpc != null)
                {
                    if (!ReferenceEquals(oldSource, value))
                    {
                        Subscription = inpc.ObservePropertyChanged(PathProperty.PropertyInfo.Name, !isNullToNull)
                                           .Subscribe(_onNext, _onError);
                    }
                }
                else
                {
                    Subscription = null;
                    if (!isNullToNull)
                    {
                        OnPropertyChanged(value, PropertyChangedEventArgs);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the subscription.
        /// </summary>
        public IDisposable Subscription
        {
            get { return _subscription; }
            private set
            {
                _subscription.Disposable = value;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Subscription.Dispose();
        }

        private void OnError(Exception obj)
        {
            throw new NotImplementedException();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var next = Next;
            if (next != null)
            {
                var source = Source;
                var value = source != null
                                ? (INotifyPropertyChanged)PathProperty.PropertyInfo.GetValue(Source)
                                : null;
                if (ReferenceEquals(value, next.Source) && value != null) // The source signaled event without changing value. We still bubble up since it is not our job to filter.
                {
                    next.OnPropertyChanged(next.Source, e);
                }
                else if (string.IsNullOrEmpty(e.PropertyName) && value != null) // We want eventArgs.PropertyName string.Empty to bubble up
                {
                    next.OnPropertyChanged(next.Source, e);
                }
                else
                {
                    next.Source = value; // Let event bubble up this way.
                }
            }

            PropertyChanged?.Invoke(sender, e);
        }

        private bool IsNullToNull(object oldSource, object newSource)
        {
            var propertyInfo = PathProperty.PropertyInfo;
            var oldValue = oldSource != null ? propertyInfo.GetValue(oldSource) : null;
            var newValue = newSource != null ? propertyInfo.GetValue(newSource) : null;
            return oldValue == null && newValue == null;
        }
    }
}