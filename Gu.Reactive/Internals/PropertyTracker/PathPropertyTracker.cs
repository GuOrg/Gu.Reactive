namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    [DebuggerDisplay("{this.PathProperty}")]
    internal sealed class PathPropertyTracker : IDisposable
    {
        private readonly PropertyPathTracker pathTracker;
        private readonly PropertyChangedEventHandler onTrackedPropertyChanged;
        private readonly object gate = new object();

        private INotifyPropertyChanged source;
        private bool disposed;

        public PathPropertyTracker(PropertyPathTracker pathTracker, PathProperty pathProperty)
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

            this.pathTracker = pathTracker;
            this.PathProperty = pathProperty;
            this.onTrackedPropertyChanged = (o, e) =>
                {
                    if (NotifyPropertyChangedExt.IsMatch(e, this.PathProperty.PropertyInfo))
                    {
                        this.OnTrackedPropertyChanged(o, e);
                        this.pathTracker.Refresh();
                    }
                };
        }

        public event TrackedPropertyChangedEventHandler TrackedPropertyChanged;

        public PathProperty PathProperty { get; }

        public PropertyChangedEventArgs PropertyChangedEventArgs => CachedEventArgs.GetOrCreatePropertyChangedEventArgs(this.PathProperty.PropertyInfo.Name);

        public PathPropertyTracker Next => this.pathTracker.GetNext(this);

        public PathPropertyTracker Previous => this.pathTracker.GetPrevious(this);

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
                    var previous = this.Previous;
                    this.OnTrackedPropertyChanged(
                        previous?.source,
                        previous?.PropertyChangedEventArgs ??
                        CachedEventArgs.GetOrCreatePropertyChangedEventArgs(string.Empty));
                }
                else if (oldSource != null)
                {
                    var previous = this.Previous;
                    this.OnTrackedPropertyChanged(
                        previous?.source,
                        previous?.PropertyChangedEventArgs ??
                        CachedEventArgs.GetOrCreatePropertyChangedEventArgs(string.Empty));
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

        private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var next = this.Next;
            if (next != null)
            {
                var value = this.PathProperty.GetPropertyValue(this.source);
                if (ReferenceEquals(value.GetValueOrDefault(), next.Source) && value.GetValueOrDefault() != null)
                {
                    // The source signaled event without changing value.
                    // We still bubble up since it is not our job to filter.
                    next.OnTrackedPropertyChanged(next.Source, e);
                }
                else
                {
                    next.Source = (INotifyPropertyChanged)value.GetValueOrDefault(); // Let event bubble up this way.
                }
            }

            this.TrackedPropertyChanged?.Invoke(sender, this.source, e);
        }
    }
}