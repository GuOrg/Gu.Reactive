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

    [DebuggerDisplay("Count = {Current.Count}")]
    public class CollectionSynchronizer<T> : IReadOnlyList<T>
    {
        public static readonly IReadOnlyList<NotifyCollectionChangedEventArgs> EmptyArgs = new NotifyCollectionChangedEventArgs[0];
        public static readonly IReadOnlyList<NotifyCollectionChangedEventArgs> ResetArgs = new[] { Diff.NotifyCollectionResetEventArgs };
        private readonly List<T> inner = new List<T>();

        public CollectionSynchronizer(IEnumerable<T> source)
        {
            this.inner.AddRange(source ?? Enumerable.Empty<T>());
        }

        public IReadOnlyList<T> Current
        {
            get
            {
                lock (this.inner)
                {
                    return this.inner;
                }
            }
        }

        public void Reset(
            object sender,
            IReadOnlyList<T> updated,
            IScheduler scheduler,
            PropertyChangedEventHandler propertyChanged,
            NotifyCollectionChangedEventHandler collectionChanged)
        {
            this.Refresh(sender, updated, EmptyArgs, scheduler, propertyChanged, collectionChanged);
        }

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

        #region IList & IList<T>

        public int Count => this.inner.Count;

        public object SyncRoot => ((IList)this.inner).SyncRoot;

        public bool IsSynchronized => ((IList)this.inner).IsSynchronized;

        public T this[int index] => this.inner[index];

        public IEnumerator<T> GetEnumerator() => this.inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public int IndexOf(T value)
        {
            lock (this.inner)
            {
                return this.inner.IndexOf(value);
            }
        }

        public int IndexOf(object value)
        {
            lock (this.inner)
            {
                return ((IList)this.inner).IndexOf(value);
            }
        }

        public int LastIndexOf(T value)
        {
            lock (this.inner)
            {
                return this.inner.LastIndexOf(value);
            }
        }

        public bool Contains(T value)
        {
            lock (this.inner)
            {
                return this.inner.Contains(value);
            }
        }

        public bool Contains(object value)
        {
            lock (this.inner)
            {
                return ((IList)this.inner).Contains(value);
            }
        }

        public void CopyTo(T[] array, int index)
        {
            lock (this.inner)
            {
                this.inner.CopyTo(array, index);
            }
        }

        public void CopyTo(Array array, int index)
        {
            lock (this.inner)
            {
                ((IList)this.inner).CopyTo(array, index);
            }
        }

        #endregion Ilist & IList<T>
    }
}