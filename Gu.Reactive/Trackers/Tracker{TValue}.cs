namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Internals;

    /// <summary>
    /// A base class for trackers of aggregates of the values in a collection.
    /// </summary>
    public abstract class Tracker<TValue> : ITracker<TValue?>
        where TValue : struct
    {
        private readonly IChanges<TValue> source;

        private TValue? value;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tracker{TValue}"/> class.
        /// </summary>
        protected Tracker(IChanges<TValue> source)
        {
            Ensure.NotNull(source, nameof(source));
            this.source = source;
            source.Add += this.OnAdd;
            source.Remove += this.OnRemove;
            source.Reset += this.OnReset;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public TValue? Value
        {
            get
            {
                this.ThrowIfDisposed();
                return this.value;
            }

            protected set
            {
                if (Nullable.Equals(value, this.value))
                {
                    return;
                }

                this.value = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Reset calculation of <see cref="Value"/>
        /// </summary>
        public virtual void Reset()
        {
            this.ThrowIfDisposed();
            this.OnReset(this.source.Values);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.source.Add -= this.OnAdd;
                this.source.Remove -= this.OnRemove;
                this.source.Reset -= this.OnReset;
                this.source.Dispose();
            }
        }

        /// <summary>
        /// Called when the source collection notifies about collection changes.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        protected virtual void OnReset(IEnumerable<TValue> values)
        {
            var retry = 0;
            while (true)
            {
                try
                {
                    this.Value = this.GetValueOrDefault(values);
                    break;
                }
                catch (InvalidOperationException e) when (e.Message == Exceptions.CollectionWasModified.Message &&
                                                          retry < 5)
                {
                    retry++;
                }
            }
        }

        /// <summary>
        /// Called when a value is added to the source collection.
        /// </summary>
        /// <param name="value">The new value.</param>
        protected abstract void OnAdd(TValue value);

        /// <summary>
        /// Called when a value is removed from the source collection.
        /// </summary>
        /// <param name="value">The removed value.</param>
        protected abstract void OnRemove(TValue value);

        /// <summary>
        /// Produce a <see cref="Value"/> from the source collection.
        /// Must handle empty collection.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <returns>The value.</returns>
        protected abstract TValue? GetValueOrDefault(IEnumerable<TValue> source);

        /// <summary>
        /// Check if the instance is disposed and throw ObjectDisposedException if it is.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="Tracker{TValue}"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}