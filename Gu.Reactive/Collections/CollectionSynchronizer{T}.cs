namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Concurrency;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Current.Count}")]
    public class CollectionSynchronizer<T> : IReadOnlyList<T>
    {
        internal static readonly IReadOnlyList<NotifyCollectionChangedEventArgs> ResetArgs = new[] { Diff.NotifyCollectionResetEventArgs };
        private static readonly IReadOnlyList<NotifyCollectionChangedEventArgs> EmptyArgs = new NotifyCollectionChangedEventArgs[0];
        private readonly List<T> inner = new List<T>();

        public CollectionSynchronizer(IEnumerable<T> source)
        {
            this.inner.AddRange(source ?? Enumerable.Empty<T>());
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
        public IEnumerator<T> GetEnumerator() => this.inner.GetEnumerator();

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
        /// <param name="sender">The sender to use when notifying.</param>
        /// <param name="updated">The updated collection.</param>
        /// <param name="scheduler">The scheduler to notify on.</param>
        /// <param name="propertyChanged">The <see cref="PropertyChangedEventHandler"/> to notify on.</param>
        /// <param name="collectionChanged">The <see cref="NotifyCollectionChangedEventHandler"/> to notify on.</param>
        public void Reset(
            object sender,
            IReadOnlyList<T> updated,
            IScheduler scheduler,
            PropertyChangedEventHandler propertyChanged,
            NotifyCollectionChangedEventHandler collectionChanged)
        {
            lock (this.SyncRoot)
            {
                this.Refresh(sender, updated, EmptyArgs, scheduler, propertyChanged, collectionChanged);
            }
        }

        /// <summary>
        /// Set <see cref="Current"/> to <paramref name="updated"/> and notify about changes.
        /// </summary>
        /// <param name="sender">The sender to use when notifying.</param>
        /// <param name="updated">The updated collection.</param>
        /// <param name="collectionChanges">The captured collection change args.</param>
        /// <param name="scheduler">The scheduler to notify on.</param>
        /// <param name="propertyChanged">The <see cref="PropertyChangedEventHandler"/> to notify on.</param>
        /// <param name="collectionChanged">The <see cref="NotifyCollectionChangedEventHandler"/> to notify on.</param>
        public void Refresh(
            object sender,
            IReadOnlyList<T> updated,
            IReadOnlyList<NotifyCollectionChangedEventArgs> collectionChanges,
            IScheduler scheduler,
            PropertyChangedEventHandler propertyChanged,
            NotifyCollectionChangedEventHandler collectionChanged)
        {
            lock (this.SyncRoot)
            {
                lock (updated.SyncRootOrDefault(this.SyncRoot))
                {
                    var change = this.Update(updated, collectionChanges, propertyChanged != null || collectionChanged != null);
                    Notifier.Notify(sender, change, scheduler, propertyChanged, collectionChanged);
                }
            }
        }

        private NotifyCollectionChangedEventArgs Update(IReadOnlyList<T> updated, IReadOnlyList<NotifyCollectionChangedEventArgs> collectionChanges, bool calculateDiff)
        {
            NotifyCollectionChangedEventArgs change = calculateDiff
                                                          ? Diff.CollectionChange(this.inner, updated, collectionChanges)
                                                          : null;
            if (!calculateDiff || change != null)
            {
                this.inner.Clear();
                this.inner.AddRange(updated);
            }

            return change;
        }
    }
}