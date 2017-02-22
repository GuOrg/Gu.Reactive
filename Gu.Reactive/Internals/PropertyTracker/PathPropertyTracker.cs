namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;

    internal sealed class PathPropertyTracker : IPathPropertyTracker
    {
        private readonly PropertyChangedEventHandler onTrackedPropertyChanged;
        private readonly object gate = new object();

        private INotifyPropertyChanged source;
        private bool disposed;

        public PathPropertyTracker(IPathPropertyTracker previous, PathProperty pathProperty)
        {
            Ensure.NotNull(pathProperty, nameof(pathProperty));
            Ensure.NotNull(pathProperty.PropertyInfo.ReflectedType, nameof(pathProperty));

            var type = pathProperty.PropertyInfo.ReflectedType;
            if (type == null)
            {
                throw new ArgumentException("PathProperty.ReflectedType == null");
            }

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
            this.onTrackedPropertyChanged = (o, e) =>
                {
                    if (NotifyPropertyChangedExt.IsMatch(e, this.PathProperty.PropertyInfo))
                    {
                        this.OnTrackedPropertyChanged(o, e);
                    }
                };

            var notifyingPathItem = previous as PathPropertyTracker;
            if (notifyingPathItem != null)
            {
                notifyingPathItem.Next = this;
            }

            if (previous != null)
            {
                this.Source = (INotifyPropertyChanged)previous.Value();
            }
        }

        public event PropertyChangedEventHandler TrackedPropertyChanged;

        public PathProperty PathProperty { get; }

        public PropertyChangedEventArgs PropertyChangedEventArgs => CachedEventArgs.GetOrCreatePropertyChangedEventArgs(this.PathProperty.PropertyInfo.Name);

        public PathPropertyTracker Next { get; private set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public INotifyPropertyChanged Source
        {
            get
            {
                return this.source;
            }

            set
            {
                if (this.disposed)
                {
                    return;
                }

                INotifyPropertyChanged oldSource;
                lock (this.gate)
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    oldSource = this.source;
                    this.source = value;
                }

                if (ReferenceEquals(oldSource, value))
                {
                    if (value != null)
                    {
                        this.OnTrackedPropertyChanged(value, this.PropertyChangedEventArgs);
                    }

                    return;
                }

                if (oldSource != null)
                {
                    oldSource.PropertyChanged -= this.onTrackedPropertyChanged;
                }

                if (value != null)
                {
                    value.PropertyChanged += this.onTrackedPropertyChanged;
                    if (!this.IsNullToNull(oldSource, value))
                    {
                        this.OnTrackedPropertyChanged(value, this.PropertyChangedEventArgs);
                    }
                }
                else
                {
                    this.OnTrackedPropertyChanged(null, this.PropertyChangedEventArgs);
                }
            }
        }

        public object Value() => this.PathProperty.GetPropertyValue(this.Source).ValueOrDefault();

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            INotifyPropertyChanged oldSource;
            lock (this.gate)
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
                oldSource = this.source;
                this.source = null;
            }

            if (oldSource != null)
            {
                oldSource.PropertyChanged -= this.onTrackedPropertyChanged;
            }
        }

        private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var next = this.Next;
            if (next != null)
            {
                var value = this.PathProperty.GetPropertyValue(this.source);
                if (ReferenceEquals(value.ValueOrDefault(), next.Source) && value.ValueOrDefault() != null)
                {
                    // The source signaled event without changing value.
                    // We still bubble up since it is not our job to filter.
                    next.OnTrackedPropertyChanged(next.Source, e);
                }
                else
                {
                    next.Source = (INotifyPropertyChanged)value.ValueOrDefault(); // Let event bubble up this way.
                }
            }

            this.TrackedPropertyChanged?.Invoke(sender, e);
        }

        private bool IsNullToNull(object oldSource, object newSource)
        {
            var oldValue = this.PathProperty.GetPropertyValue(oldSource);
            var newValue = this.PathProperty.GetPropertyValue(newSource);
            return oldValue.ValueOrDefault() == null && newValue.ValueOrDefault() == null;
        }
    }
}