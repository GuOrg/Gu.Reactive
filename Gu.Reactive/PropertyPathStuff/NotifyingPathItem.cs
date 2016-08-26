namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.ComponentModel;
    using System.Reactive;
    using System.Reactive.Disposables;

    using Gu.Reactive.Internals;

    internal sealed class NotifyingPathItem : INotifyingPathItem
    {
        private readonly WeakReference sourceRef = new WeakReference(null);
        private readonly Action<EventPattern<PropertyChangedEventArgs>> onNext;
        private readonly Action<Exception> onError;
        private readonly SerialDisposable subscription = new SerialDisposable();
        private bool disposed;

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

            this.PathProperty = pathProperty;
            this.onNext = x => this.OnPropertyChanged(x.Sender, x.EventArgs);
            this.onError = this.OnError;
            this.PropertyChangedEventArgs = new PropertyChangedEventArgs(this.PathProperty.PropertyInfo.Name);
            this.Previous = previous;
            var notifyingPathItem = previous as NotifyingPathItem;
            if (notifyingPathItem != null)
            {
                notifyingPathItem.Next = this;
            }

            if (previous != null)
            {
                this.Source = (INotifyPropertyChanged)previous.Value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PathProperty PathProperty { get; }

        public INotifyingPathItem Previous { get; private set; }

        public NotifyingPathItem Next { get; private set; }

        public PropertyChangedEventArgs PropertyChangedEventArgs { get; }

        public bool IsLast => this.PathProperty.IsLast;

        public object Value
        {
            get
            {
                var source = this.Source;
                if (source == null)
                {
                    return null;
                }

                return this.PathProperty.PropertyInfo.GetValue(source);
            }
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public INotifyPropertyChanged Source
        {
            get
            {
                return (INotifyPropertyChanged)this.sourceRef.Target;
            }

            set
            {
                var oldSource = this.sourceRef.Target;
                this.sourceRef.Target = value;
                var inpc = value;

                var isNullToNull = this.IsNullToNull(oldSource, value);
                if (inpc != null)
                {
                    if (!ReferenceEquals(oldSource, value))
                    {
                        this.Subscription = inpc.ObservePropertyChanged(this.PathProperty.PropertyInfo.Name, !isNullToNull)
                                           .Subscribe(this.onNext, this.onError);
                    }
                }
                else
                {
                    this.Subscription = null;
                    if (!isNullToNull)
                    {
                        this.OnPropertyChanged(value, this.PropertyChangedEventArgs);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the subscription.
        /// </summary>
        public IDisposable Subscription
        {
            get { return this.subscription; }

            private set
            {
                this.subscription.Disposable = value;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Subscription.Dispose();
        }

        private void OnError(Exception obj)
        {
            throw new NotImplementedException();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var next = this.Next;
            if (next != null)
            {
                var source = this.Source;
                var value = source != null
                                ? (INotifyPropertyChanged)this.PathProperty.PropertyInfo.GetValue(this.Source)
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

            this.PropertyChanged?.Invoke(sender, e);
        }

        private bool IsNullToNull(object oldSource, object newSource)
        {
            var propertyInfo = this.PathProperty.PropertyInfo;
            var oldValue = oldSource != null ? propertyInfo.GetValue(oldSource) : null;
            var newValue = newSource != null ? propertyInfo.GetValue(newSource) : null;
            return oldValue == null && newValue == null;
        }
    }
}