namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Helper for synchronizing two coillections and notifying about diffs.
    /// </summary>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Current.Count}")]
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public class CollectionSynchronizer<T> : IReadOnlyList<T>
    {
        private readonly List<T> temp = new List<T>();
        private readonly List<T> inner = new List<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionSynchronizer{T}"/> class.
        /// </summary>
        public CollectionSynchronizer(IEnumerable<T> source)
        {
            this.Reset(source ?? Enumerable.Empty<T>());
        }

        /// <summary>
        /// The current items.
        /// </summary>
        public IReadOnlyList<T> Current
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.inner;
                }
            }
        }

        /// <inheritdoc/>
        public int Count => this.inner.Count;

        /// <summary>
        /// See <see cref="ICollection.SyncRoot"/>
        /// </summary>
        public object SyncRoot => ((IList)this.inner).SyncRoot;

        /// <summary>
        /// See <see cref="ICollection.IsSynchronized"/>
        /// </summary>
        public bool IsSynchronized => ((IList)this.inner).IsSynchronized;

        /// <inheritdoc/>
        public T this[int index]
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.inner[index];
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            lock (this.inner)
            {
                return this.inner.GetEnumerator();
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// See <see cref="List{T}.IndexOf(T)"/>
        /// </summary>
        public int IndexOf(T value)
        {
            lock (this.inner)
            {
                return this.inner.IndexOf(value);
            }
        }

        /// <summary>
        /// See <see cref="IList.IndexOf(object)"/>
        /// </summary>
        public int IndexOf(object value)
        {
            lock (this.inner)
            {
                return ((IList)this.inner).IndexOf(value);
            }
        }

        /// <summary>
        /// See <see cref="List{T}.LastIndexOf(T)"/>
        /// </summary>
        public int LastIndexOf(T value)
        {
            lock (this.inner)
            {
                return this.inner.LastIndexOf(value);
            }
        }

        /// <summary>
        /// See <see cref="List{T}.Contains(T)"/>
        /// </summary>
        public bool Contains(T value)
        {
            lock (this.inner)
            {
                return this.inner.Contains(value);
            }
        }

        /// <summary>
        /// See <see cref="IList.Contains(object)"/>
        /// </summary>
        public bool Contains(object value)
        {
            lock (this.inner)
            {
                return ((IList)this.inner).Contains(value);
            }
        }

        /// <summary>
        /// See <see cref="List{T}.CopyTo(T[], int)"/>
        /// </summary>
        public void CopyTo(T[] array, int index)
        {
            lock (this.inner)
            {
                this.inner.CopyTo(array, index);
            }
        }

        /// <summary>
        /// See <see cref="ICollection.CopyTo(Array, int)"/>
        /// </summary>
        public void CopyTo(Array array, int index)
        {
            lock (this.inner)
            {
                ((IList)this.inner).CopyTo(array, index);
            }
        }

        /// <summary>
        /// Set <see cref="Current"/> to <paramref name="updated"/> and notify about changes.
        /// </summary>
        /// <param name="updated">The updated collection.</param>
        public void Reset(IEnumerable<T> updated)
        {
            lock (this.SyncRoot)
            {
                this.Refresh(updated, CachedEventArgs.EmptyArgs, null, null);
            }
        }

        /// <summary>
        /// Set <see cref="Current"/> to <paramref name="updated"/> and notify about changes.
        /// </summary>
        /// <param name="updated">The updated collection.</param>
        /// <param name="propertyChanged">The <see cref="Action{PropertyChangedEventArgs}"/> to notify on.</param>
        /// <param name="collectionChanged">The <see cref="Action{NotifyCollectionChangedEventArgs}"/> to notify on.</param>
        public void Reset(IEnumerable<T> updated, Action<PropertyChangedEventArgs> propertyChanged, Action<NotifyCollectionChangedEventArgs> collectionChanged)
        {
            lock (this.SyncRoot)
            {
                this.Refresh(updated, CachedEventArgs.EmptyArgs, propertyChanged, collectionChanged);
            }
        }

        /// <summary>
        /// Set <see cref="Current"/> to <paramref name="updated"/> and notify about changes.
        /// </summary>
        /// <param name="updated">The updated collection.</param>
        /// <param name="collectionChanges">The captured collection change args.</param>
        /// <param name="propertyChanged">The <see cref="Action{PropertyChangedEventArgs}"/> to notify on.</param>
        /// <param name="collectionChanged">The <see cref="Action{NotifyCollectionChangedEventArgs}"/> to notify on.</param>
        public void Refresh(IEnumerable<T> updated, IReadOnlyList<NotifyCollectionChangedEventArgs> collectionChanges = null, Action<PropertyChangedEventArgs> propertyChanged = null, Action<NotifyCollectionChangedEventArgs> collectionChanged = null)
        {
            lock (this.SyncRoot)
            {
                lock (updated.SyncRootOrDefault(this.SyncRoot))
                {
                    var change = this.Update(updated, collectionChanges, propertyChanged, collectionChanged);
                    if (change != null)
                    {
                        switch (change.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                            case NotifyCollectionChangedAction.Remove:
                                propertyChanged?.Invoke(CachedEventArgs.CountPropertyChanged);
                                propertyChanged?.Invoke(CachedEventArgs.IndexerPropertyChanged);
                                collectionChanged?.Invoke(change);
                                break;
                            case NotifyCollectionChangedAction.Replace:
                            case NotifyCollectionChangedAction.Move:
                                propertyChanged?.Invoke(CachedEventArgs.IndexerPropertyChanged);
                                collectionChanged?.Invoke(change);
                                break;
                            case NotifyCollectionChangedAction.Reset:
                                propertyChanged?.Invoke(CachedEventArgs.CountPropertyChanged);
                                propertyChanged?.Invoke(CachedEventArgs.IndexerPropertyChanged);
                                collectionChanged?.Invoke(change);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        private NotifyCollectionChangedEventArgs Update(IEnumerable<T> updated, IReadOnlyList<NotifyCollectionChangedEventArgs> collectionChanges, Action<PropertyChangedEventArgs> propertyChanged, Action<NotifyCollectionChangedEventArgs> collectionChanged)
        {
            lock (this.inner)
            {
                // retrying five times here if collection is modified.
                for (var i = 0; i < 5; i++)
                {
                    this.temp.Clear();
                    try
                    {
                        this.temp.AddRange(updated);
                        break;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }

                if (propertyChanged != null || collectionChanged != null)
                {
                    var change = Diff.CollectionChange(this.inner, this.temp, collectionChanges);
                    this.inner.Clear();
                    this.inner.AddRange(this.temp);
                    this.temp.Clear();
                    return change;
                }

                this.inner.Clear();
                this.inner.AddRange(this.temp);
                this.temp.Clear();
                return null;
            }
        }
    }
}