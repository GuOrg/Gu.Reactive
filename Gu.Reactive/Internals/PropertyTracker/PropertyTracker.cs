namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    [DebuggerDisplay("{this.Getter.Property}")]
    internal sealed class PropertyTracker<TSource, TValue> : IPropertyTracker<TValue>
        where TSource : class, INotifyPropertyChanged
    {
        private readonly PropertyChangedEventHandler onTrackedPropertyChanged;
        private readonly object gate = new object();

        private TSource source;
        private bool disposed;

        public PropertyTracker(IPropertyPathTracker pathTracker, NotifyingGetter<TSource, TValue> getter)
        {
            Ensure.NotNull(pathTracker, nameof(pathTracker));
            Ensure.NotNull(getter, nameof(getter));

            Ensure.NotNull(getter.Property.ReflectedType, nameof(getter));
            var type = getter.Property.ReflectedType;
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
                    getter.Property.Name);
                throw new ArgumentException(message, nameof(getter));
            }

            if (!typeof(INotifyPropertyChanged).IsAssignableFrom(type))
            {
                var message = string.Format(
                    "All levels in the path must implement INotifyPropertyChanged." + Environment.NewLine +
                    "The type {0}.{1} does not so the property {1}.{2} will not notify when value changes.",
                    type.Namespace,
                    type.PrettyName(),
                    getter.Property.Name);
                throw new ArgumentException(message, nameof(getter));
            }

            this.PathTracker = pathTracker;
            this.Getter = getter;
            this.onTrackedPropertyChanged = (o, e) =>
                {
                    if (NotifyPropertyChangedExt.IsMatch(e, this.Getter.Property))
                    {
                        this.OnTrackedPropertyChanged(o, e);
                    }
                };
        }

        public event TrackedPropertyChangedEventHandler<TValue> TrackedPropertyChanged;

        event PropertyChangedEventHandler IPropertyTracker.TrackedPropertyChanged
        {
            add { this.TrackedPropertyChangedInternal += value; }
            remove { this.TrackedPropertyChangedInternal -= value; }
        }

        private event PropertyChangedEventHandler TrackedPropertyChangedInternal;

        public IPropertyPathTracker PathTracker { get; }

        public Getter<TSource, TValue> Getter { get; }

        IGetter IPropertyTracker.Getter => this.Getter;

        public IPropertyTracker Next => this.PathTracker.GetNext(this);

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public TSource Source
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

        INotifyPropertyChanged IPropertyTracker.Source
        {
            get { return this.source; }
            set { this.Source = (TSource)value; }
        }

        Maybe<TValue> IPropertyTracker<TValue>.GetMaybe() => this.Getter.GetMaybe(this.source);

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

        void IPropertyTracker.OnTrackedPropertyChanged(object sender, INotifyPropertyChanged newSource, PropertyChangedEventArgs e) => this.OnTrackedPropertyChanged(sender, newSource, e);

        private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.TrackedPropertyChangedInternal?.Invoke(sender, e);
            this.OnTrackedPropertyChanged(sender, this.source, e);
        }

        private void OnTrackedPropertyChanged(object sender, INotifyPropertyChanged newSource, PropertyChangedEventArgs e)
        {
            this.Source = (TSource)newSource;
            var value = this.Getter.GetMaybe(this.source);
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