namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;

    [DebuggerDisplay("{this.Getter.Property}")]
    internal sealed class PropertyTracker<TSource, TValue> : IPropertyTracker<TValue>, INotifyPropertyChanged
        where TSource : class, INotifyPropertyChanged
    {
        private readonly PropertyChangedEventHandler onTrackedPropertyChanged;
        private readonly object gate = new object();

        private TSource? source;
        private bool disposed;

        internal PropertyTracker(IPropertyPathTracker pathTracker, NotifyingGetter<TSource, TValue> getter)
        {
            var type = getter.Property.ReflectedType;
            if (type is null)
            {
                throw new ArgumentException("getter.Property.ReflectedType is null");
            }

            if (type.IsValueType)
            {
                var message = $"Property path cannot have structs in it. Copy by value will make subscribing error prone.{Environment.NewLine}" +
                                    $"The type {type.Namespace}.{type.PrettyName()} is a value type not so {type.PrettyName()}.{getter.Property.Name} subscribing to changes is weird.";
                throw new ArgumentException(message, nameof(getter));
            }

            if (!typeof(INotifyPropertyChanged).IsAssignableFrom(type))
            {
                var message = $"All levels in the path must implement INotifyPropertyChanged.{Environment.NewLine}" +
                                    $"The type {type.Namespace}.{type.PrettyName()} does not so the property {type.PrettyName()}.{getter.Property.Name} will not notify when value changes.";
                throw new ArgumentException(message, nameof(getter));
            }

            this.PathTracker = pathTracker;
            this.Getter = getter;
            this.onTrackedPropertyChanged = (o, e) =>
            {
                if (e.IsMatch(getter.Property))
                {
                    this.OnTrackedPropertyChanged(o, e);
                }
            };
        }

        public event TrackedPropertyChangedEventHandler<TValue>? TrackedPropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        event PropertyChangedEventHandler IPropertyTracker.TrackedPropertyChanged
        {
            add => this.TrackedPropertyChangedInternal += value;
            remove => this.TrackedPropertyChangedInternal -= value;
        }

        private event PropertyChangedEventHandler? TrackedPropertyChangedInternal;

        public IPropertyPathTracker PathTracker { get; }

        public Getter<TSource?, TValue> Getter { get; }

        IGetter IPropertyTracker.Getter => this.Getter;

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        public TSource? Source
        {
            get => this.source;

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
                    this.OnPropertyChanged();
                }
            }
        }

        INotifyPropertyChanged? IPropertyTracker.Source
        {
            get => this.source;
            set => this.Source = (TSource?)value;
        }

        internal IPropertyTracker? Next => this.PathTracker.GetNext(this);

        Maybe<TValue> IPropertyTracker<TValue>.GetMaybe() => this.Getter.GetMaybe(this.source);

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            INotifyPropertyChanged? oldSource;
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

        void IPropertyTracker.OnTrackedPropertyChanged(object sender, INotifyPropertyChanged? newSource, PropertyChangedEventArgs e) => this.OnTrackedPropertyChanged(sender, newSource, e);

        private void OnTrackedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.TrackedPropertyChangedInternal?.Invoke(sender, e);
            this.OnTrackedPropertyChanged(sender, this.source, e);
        }

        private void OnTrackedPropertyChanged(object sender, INotifyPropertyChanged? newSource, PropertyChangedEventArgs e)
        {
            this.Source = (TSource?)newSource;
            var value = this.Getter.GetMaybe(this.source);
            var next = this.Next;
            if (next != null)
            {
                var nextSource = (INotifyPropertyChanged?)value.GetValueOrDefault();
                if (next.Source != null ||
                    nextSource != null)
                {
                    next.OnTrackedPropertyChanged(sender, nextSource, e);
                }
            }

            this.TrackedPropertyChanged?.Invoke(this, sender, e, SourceAndValue.Create(newSource, value));
        }

        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
