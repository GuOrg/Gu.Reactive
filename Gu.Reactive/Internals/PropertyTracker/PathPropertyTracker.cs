namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;
    using System.Reactive;
    using System.Reactive.Disposables;

    internal sealed class PathPropertyTracker : IPathPropertyTracker
    {
        private readonly WeakReference sourceRef = new WeakReference(null);
        private readonly Action<EventPattern<PropertyChangedEventArgs>> onNext;
        private readonly SerialDisposable subscription = new SerialDisposable();
        private bool disposed;

        public PathPropertyTracker(IPathPropertyTracker previous, PathProperty pathProperty)
        {
            Ensure.NotNull(pathProperty, nameof(pathProperty));
            Ensure.NotNull(pathProperty.PropertyInfo.ReflectedType, nameof(pathProperty));

            var type = pathProperty.PropertyInfo.ReflectedType;
            if (type.IsValueType)
            {
                var message = string.Format(
                    "Property path cannot have structs in it. Copy by value will make subscribing error prone." + Environment.NewLine +
                    "The type {0}.{1} is a value type not so {1}.{2} subscribing to changes is weird.",
                    type.Namespace,
                    type.PrettyName(),
                    pathProperty.PropertyInfo.Name);
                throw new ArgumentException(message, nameof(pathProperty));
            }

            if (!typeof(INotifyPropertyChanged).IsAssignableFrom(type))
            {
                var message = string.Format(
                    "All levels in the path must implement INotifyPropertyChanged." + Environment.NewLine +
                    "The type {0}.{1} does not so the property {1}.{2} will not notify when value changes.",
                    type.Namespace,
                    type.PrettyName(),
                    pathProperty.PropertyInfo.Name);
                throw new ArgumentException(message, nameof(pathProperty));
            }

            this.PathProperty = pathProperty;
            this.onNext = x => this.OnPropertyChanged(x.Sender, x.EventArgs);
            this.PropertyChangedEventArgs = CachedEventArgs.GetOrCreatePropertyChangedEventArgs(this.PathProperty.PropertyInfo.Name);
            var notifyingPathItem = previous as PathPropertyTracker;
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

        public PropertyChangedEventArgs PropertyChangedEventArgs { get; }

        public object Value => this.PathProperty.GetPropertyValue(this.Source).ValueOrDefault();

        public PathPropertyTracker Next { get; private set; }

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
                        this.subscription.Disposable = inpc.ObservePropertyChanged(this.PathProperty.PropertyInfo.Name, !isNullToNull)
                                                           .Subscribe(this.onNext);
                    }
                }
                else
                {
                    this.subscription.Disposable = null;
                    if (!isNullToNull)
                    {
                        this.OnPropertyChanged(value, this.PropertyChangedEventArgs);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.subscription.Dispose();
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var next = this.Next;
            if (next != null)
            {
                var value = this.PathProperty.GetPropertyValue(this.Source);
                if (ReferenceEquals(value.ValueOrDefault(), next.Source) && value.ValueOrDefault() != null)
                {
                    // The source signaled event without changing value.
                    // We still bubble up since it is not our job to filter.
                    next.OnPropertyChanged(next.Source, e);
                }
                else
                {
                    next.Source = (INotifyPropertyChanged)value.ValueOrDefault(); // Let event bubble up this way.
                }
            }

            this.PropertyChanged?.Invoke(sender, e);
        }

        private bool IsNullToNull(object oldSource, object newSource)
        {
            var oldValue = this.PathProperty.GetPropertyValue(oldSource);
            var newValue = this.PathProperty.GetPropertyValue(newSource);
            return oldValue.ValueOrDefault() == null && newValue.ValueOrDefault() == null;
        }
    }
}