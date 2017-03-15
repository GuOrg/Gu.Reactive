namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using Gu.Reactive.Internals;
    using System.Runtime.CompilerServices;

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
            Ensure.NotNull(source, nameof(source));
            this.source = source;
            source.Add += this.OnAdd;
            source.Remove += this.OnRemove;
            source.Reset += this.OnReset;
            this.Reset();
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        public TValue? Min
        {
            get
            {
                return this.min;
            }

            set
            {
                if (Nullable.Equals(value, this.min))
                {
                    return;
                }

                this.min = value;
                this.OnPropertyChanged();
            }
        }

        public TValue? Max
        {
            get
            {
                return this.max;
            }

            set
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
        /// Reset calculation of <see cref="Min"/> and <see cref="Max"/>
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
            this.source.Dispose();
        }

        private void OnAdd(TValue value)
        {
            var currentMin = this.min;
            var currentMax = this.max;
            if (currentMin == null || 
                currentMax == null)
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
            if (currentMin == null ||
                currentMax == null)
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
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private void OnReset(IEnumerable<TValue> values)
        {
            var retry = 0;
            while (true)
            {
                try
                {
                    var comparer = Comparer<TValue>.Default;
                    TValue? min = null;
                    TValue? max = null;
                    foreach (var x in values)
                    {
                        if (min != null)
                        {
                            if (comparer.Compare(x, min.Value) < 0)
                            {
                                min = x;
                            }
                        }
                        else
                        {
                            min = x;
                        }

                        if (max != null)
                        {
                            if (comparer.Compare(x, max.Value) > 0)
                            {
                                max = x;
                            }
                        }
                        else
                        {
                            max = x;
                        }
                    }

                    this.Min = min;
                    this.Max = max;
                    break;
                }
                catch (InvalidOperationException e) when (e.Message == Exceptions.CollectionWasModified.Message &&
                                                          retry < 5)
                {
                    retry++;
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}