namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    [DebuggerDisplay("{this.Property}")]
    internal sealed class PathPropertyTracker<TSource, TValue> : IPathPropertyTracker
    {
        private readonly PropertyChangedEventHandler onTrackedPropertyChanged;
        private readonly object gate = new object();

        private INotifyPropertyChanged source;
        private bool disposed;

        public PathPropertyTracker(PropertyPathTracker pathTracker, PathProperty<TSource, TValue> property)
        {
            Ensure.NotNull(property, nameof(property));
            Ensure.NotNull(property.PropertyInfo.ReflectedType, nameof(property));
            var type = property.PropertyInfo.ReflectedType;
            if (type == null)
            {
                throw new ArgumentException("PathProperty.ReflectedType == null");
            }

            if (type.IsValueType)
            {
                var message = string.Format(
                    "Property path cannot have structs in it. Copy by value will make subscribing error prone." +
                    Environment.NewLine +
                    "The type {0}.{1} is a value type not so {1}.{2} subscribing to changes is weird.",
                    type.Namespace,
                    type.PrettyName(),
                    property.PropertyInfo.Name);
                throw new ArgumentException(message, nameof(property));
            }

            if (!typeof(INotifyPropertyChanged).IsAssignableFrom(type))
            {
                var message = string.Format(
                    "All levels in the path must implement INotifyPropertyChanged." + Environment.NewLine +
                    "The type {0}.{1} does not so the property {1}.{2} will not notify when value changes.",
                    type.Namespace,
                    type.PrettyName(),
                    property.PropertyInfo.Name);
                throw new ArgumentException(message, nameof(property));
            }

            this.PathTracker = pathTracker;
            this.Property = property;
            this.onTrackedPropertyChanged = (o, e) =>
                {
                    if (NotifyPropertyChangedExt.IsMatch(e, this.Property.PropertyInfo))
                    {
                        this.OnTrackedPropertyChanged(o, e);
                        this.PathTracker.Refresh();
                    }
                };
        }

        public event TrackedPropertyChangedEventHandler TrackedPropertyChanged;

        public PropertyPathTracker PathTracker { get; }

        public IPathProperty Property { get; }

        public IPathPropertyTracker Next => this.PathTracker.GetNext(this);

        public IPathPropertyTracker Previous => this.PathTracker.GetPrevious(this);

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

                lock (this.gate)
                {
                    if (this.disposed ||
                        ReferenceEquals(value, this.source))
                    {
                        return;
                    }

                    if (this.source != null)
                    {
                        this.source.PropertyChanged -= this.onTrackedPropertyChanged;
                    }

                    if (value != null)
                    {
                        value.PropertyChanged += this.onTrackedPropertyChanged;
                    }

                    this.source = value;
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

        void IPathPropertyTracker.OnTrackedPropertyChanged(object sender, INotifyPropertyChanged newSource, PropertyChangedEventArgs e) => this.OnTrackedPropertyChanged(sender, newSource, e);

        private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnTrackedPropertyChanged(sender, this.source, e);
        }

        private void OnTrackedPropertyChanged(object sender, INotifyPropertyChanged newSource, PropertyChangedEventArgs e)
        {
            this.Source = newSource;
            var value = this.Property.Getter.GetMaybe(this.source);
            var next = this.Next;
            if (next != null)
            {
                var nextSource = (INotifyPropertyChanged)value.GetValueOrDefault();
                if (next.Source != null ||
                    nextSource != null)
                {
                    next.OnTrackedPropertyChanged(sender, nextSource, e);
                }
            }

            this.TrackedPropertyChanged?.Invoke(this, sender, e, SourceAndValue.Create(newSource, value));
        }
    }
}