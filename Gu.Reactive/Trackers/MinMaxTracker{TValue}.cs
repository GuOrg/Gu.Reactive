namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Internals;

    /// <summary>
    /// A tracker for minimum and maximum value in a collection.
    /// </summary>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    public sealed class MinMaxTracker<TValue> : INotifyPropertyChanged, IDisposable
        where TValue : struct, IComparable<TValue>
    {
        private readonly IChanges<TValue> source;
        private bool disposed;
        private TValue? min;
        private TValue? max;

        /// <summary>
        /// Initializes a new instance of the <see cref="MinMaxTracker{TValue}"/> class.
        /// </summary>
        /// <param name="source">The changes of the source collection.</param>
        public MinMaxTracker(IChanges<TValue> source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            source.Add += this.OnAdd;
            source.Remove += this.OnRemove;
            source.Reset += this.OnReset;
            this.Reset();
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the minimum value or null if the collection is empty.
        /// </summary>
        public TValue? Min
        {
            get => this.min;

            private set
            {
                if (Nullable.Equals(value, this.min))
                {
                    return;
                }

                this.min = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the maximum value or null if the collection is empty.
        /// </summary>
        public TValue? Max
        {
            get => this.max;

            private set
            {
                if (Nullable.Equals(value, this.max))
                {
                    return;
                }

                this.max = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Reset calculation of <see cref="Min"/> and <see cref="Max"/>.
        /// </summary>
        public void Reset()
        {
            this.ThrowIfDisposed();
            this.OnReset(this.source.Values);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.source.Add -= this.OnAdd;
            this.source.Remove -= this.OnRemove;
            this.source.Reset -= this.OnReset;
#pragma warning disable IDISP007 // Don't dispose injected.
            this.source.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
        }

        private void OnAdd(TValue value)
        {
            var currentMin = this.min;
            var currentMax = this.max;
            if (currentMin is null ||
                currentMax is null)
            {
                this.Min = value;
                this.Max = value;
                return;
            }

            if (Comparer<TValue>.Default.Compare(value, currentMin.Value) < 0)
            {
                this.Min = value;
            }

            if (Comparer<TValue>.Default.Compare(value, currentMax.Value) > 0)
            {
                this.Max = value;
            }
        }

        private void OnRemove(TValue value)
        {
            var currentMin = this.min;
            var currentMax = this.max;
            if (currentMin is null ||
                currentMax is null)
            {
                return;
            }

            if (Comparer<TValue>.Default.Compare(value, currentMin.Value) == 0 ||
                Comparer<TValue>.Default.Compare(value, currentMax.Value) == 0)
            {
                this.Reset();
            }
        }

        /// <summary>
        /// Called when the source collection notifies about collection changes.
        /// </summary>
        private void OnReset(IEnumerable<TValue> values)
        {
            var retry = 0;
            while (true)
            {
                try
                {
                    var comparer = Comparer<TValue>.Default;
                    TValue? tempMin = null;
                    TValue? tempMax = null;
                    foreach (var x in values)
                    {
                        if (tempMin != null)
                        {
                            if (comparer.Compare(x, tempMin.Value) < 0)
                            {
                                tempMin = x;
                            }
                        }
                        else
                        {
                            tempMin = x;
                        }

                        if (tempMax != null)
                        {
                            if (comparer.Compare(x, tempMax.Value) > 0)
                            {
                                tempMax = x;
                            }
                        }
                        else
                        {
                            tempMax = x;
                        }
                    }

                    this.Min = tempMin;
                    this.Max = tempMax;
                    break;
                }
                catch (InvalidOperationException e) when (e.Message == Exceptions.CollectionWasModified.Message &&
                                                          retry < 5)
                {
                    retry++;
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Check if the instance is disposed and throw ObjectDisposedException if it is.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(typeof(MinMaxTracker<TValue>).FullName);
            }
        }
    }
}
