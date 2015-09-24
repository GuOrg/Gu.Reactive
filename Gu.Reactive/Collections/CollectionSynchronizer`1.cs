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
        private readonly List<T> _inner = new List<T>();
        public CollectionSynchronizer(IEnumerable<T> source)
        {
            _inner.AddRange(source ?? Enumerable.Empty<T>());
        }

        public IReadOnlyList<T> Current
        {
            get
            {
                lock (_inner)
                {
                    return _inner;
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
            Refresh(sender, updated, EmptyArgs, scheduler, propertyChanged, collectionChanged);
        }

        public void Refresh(
            object sender,
            IReadOnlyList<T> updated,
            IReadOnlyList<NotifyCollectionChangedEventArgs> collectionChanges,
            IScheduler scheduler,
            PropertyChangedEventHandler propertyChanged,
            NotifyCollectionChangedEventHandler collectionChanged)
        {
            lock (_inner)
            {
                var change = Update(updated, collectionChanges, propertyChanged != null || collectionChanged != null);
                Notifier.Notify(sender, change, scheduler, propertyChanged, collectionChanged);
            }
        }

        private NotifyCollectionChangedEventArgs Update(IReadOnlyList<T> updated, IReadOnlyList<NotifyCollectionChangedEventArgs> collectionChanges, bool calculateDiff)
        {
            NotifyCollectionChangedEventArgs change = calculateDiff
                                                          ? Diff.CollectionChange(_inner, updated, collectionChanges)
                                                          : null;
            if (!calculateDiff || change != null)
            {
                _inner.Clear();
                _inner.AddRange(updated);
            }

            return change;
        }

        #region IList & IList<T>

        public int Count => _inner.Count;

        public object SyncRoot => ((IList)_inner).SyncRoot;

        public bool IsSynchronized => ((IList)_inner).IsSynchronized;

        public T this[int index] => _inner[index];

        public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(T value)
        {
            lock (_inner)
            {
                return _inner.IndexOf(value);
            }
        }

        public int IndexOf(object value)
        {
            lock (_inner)
            {
                return ((IList)_inner).IndexOf(value);
            }
        }

        public int LastIndexOf(T value)
        {
            lock (_inner)
            {
                return _inner.LastIndexOf(value);
            }
        }

        public bool Contains(T value)
        {
            lock (_inner)
            {
                return _inner.Contains(value);
            }
        }

        public bool Contains(object value)
        {
            lock (_inner)
            {
                return ((IList)_inner).Contains(value);
            }
        }

        public void CopyTo(T[] array, int index)
        {
            lock (_inner)
            {
                _inner.CopyTo(array, index);
            }
        }

        public void CopyTo(Array array, int index)
        {
            lock (_inner)
            {
                ((IList)_inner).CopyTo(array, index);
            }
        }

        #endregion Ilist & IList<T>
    }
}