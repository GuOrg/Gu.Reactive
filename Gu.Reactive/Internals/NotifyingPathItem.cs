namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    internal sealed class NotifyingPathItem : PathItem, IDisposable, INotifyPropertyChanged
    {
        internal readonly PropertyChangedEventArgs PropertyChangedEventArgs;
        private bool _disposed;
        private IDisposable _subscription;

        public NotifyingPathItem(NotifyingPathItem previous, PropertyInfo propertyInfo)
            : base(previous, propertyInfo)
        {
            PropertyChangedEventArgs = new PropertyChangedEventArgs(PropertyInfo.Name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
                    if (value.GetType() != PropertyInfo.DeclaringType)
                    {
                        throw new InvalidOperationException(string.Format("Trying to set source to illegal type. Was: {0} expected {1}", value.GetType().FullName, PropertyInfo.DeclaringType.FullName));
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
                        Subscription = inpc.ObservePropertyChanged(PropertyInfo.Name, !isNullToNull)
                                           .Subscribe(x => OnPropertyChanged(x.Sender, x.EventArgs));
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

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var next = (NotifyingPathItem)Next;
            if (next != null)
            {
                var value = (INotifyPropertyChanged)Value;
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
            var oldValue = oldSource != null ? PropertyInfo.GetValue(oldSource) : null;
            var newValue = newSource != null ? PropertyInfo.GetValue(newSource) : null;
            return oldValue == null && newValue == null;
        }
    }
}