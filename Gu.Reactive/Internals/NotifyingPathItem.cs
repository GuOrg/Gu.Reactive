namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;
    using System.Reactive;
    using System.Reflection;

    internal sealed class NotifyingPathItem : INotifyingPathItem
    {
        private readonly PropertyChangedEventArgs _propertyChangedEventArgs;
        private readonly WeakReference _sourceRef = new WeakReference(null);
        private bool _disposed;
        private IDisposable _subscription;
        private readonly Action<EventPattern<PropertyChangedEventArgs>> _onNext;

        private readonly Action<Exception> _onError;

        private NotifyingPathItem()
        {
        }

        public NotifyingPathItem(INotifyingPathItem previous, PathItem pathItem)
        {
            PathItem = pathItem;
            _onNext = x => OnPropertyChanged(x.Sender, x.EventArgs);
            _onError = OnError;
            _propertyChangedEventArgs = new PropertyChangedEventArgs(PathItem.PropertyInfo.Name);
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

        public PathItem PathItem { get; private set; }

        public INotifyingPathItem Previous { get; private set; }

        public NotifyingPathItem Next { get; private set; }

        public PropertyChangedEventArgs PropertyChangedEventArgs
        {
            get { return _propertyChangedEventArgs; }
        }

        public bool IsLast
        {
            get { return PathItem.IsLast; }
        }

        public object Value
        {
            get { return PathItem.Value; }
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
                if (value != null)
                {
                    if (value.GetType() != PathItem.PropertyInfo.DeclaringType)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                "Trying to set source to illegal type. Was: {0} expected {1}",
                                value.GetType().FullName,
                               PathItem.PropertyInfo.DeclaringType.FullName));
                    }
                }

                var oldSource = _sourceRef.Target;
                _sourceRef.Target = value;
                var inpc = value;

                var isNullToNull = IsNullToNull(oldSource, value);
                if (inpc != null)
                {
                    if (!ReferenceEquals(oldSource, value))
                    {
                        Subscription = inpc.ObservePropertyChanged(PathItem.PropertyInfo.Name, !isNullToNull)
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
                if (_subscription != null)
                {
                    _subscription.Dispose();
                }
                _subscription = value;
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
            if (Subscription != null)
            {
                Subscription.Dispose();
            }
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
                var value = (INotifyPropertyChanged)PathItem.Value;
                next.Source = value;
            }
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        private bool IsNullToNull(object oldSource, object newSource)
        {
            var oldValue = oldSource != null ? PathItem.GetValue(oldSource) : null;
            var newValue = newSource != null ? PathItem.GetValue(newSource) : null;
            return oldValue == null && newValue == null;
        }
    }
}