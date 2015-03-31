namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;
    using System.Reactive;
    using System.Reflection;

    internal sealed class NotifyingPathItem : PathItem, IDisposable, INotifyPropertyChanged
    {
        private bool _disposed;
        private IDisposable _subscription;

        public NotifyingPathItem(NotifyingPathItem previous, PropertyInfo propertyInfo)
            : base(previous, propertyInfo)
        {
            if (previous != null)
            {
                previous.PropertyChanged += OnPropertyChanged;
            }
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
                if (ReferenceEquals(_sourceRef.Target, value))
                {
                    return;
                }
                var inpc = value;
                if (inpc != null)
                {
                    Subscription = inpc.ObservePropertyChanged(PropertyInfo.Name)
                                       .Subscribe(x => OnPropertyChanged(x.Sender, x.EventArgs));
                }
                _sourceRef.Target = value;
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
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}