namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    [Serializable()]
    public class ObservableSet<T> : IObservableSet<T>, IReadOnlyCollection<T>
    {
        private static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Count));
        private static readonly NotifyCollectionChangedEventArgs ResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        private readonly HashSet<T> _inner;

        public ObservableSet()
            : this(EqualityComparer<T>.Default)
        {
        }

        public ObservableSet(IEqualityComparer<T> comparer)
        {
            _inner = new HashSet<T>(comparer);
        }

        public ObservableSet(IEnumerable<T> collection)
            : this(collection, EqualityComparer<T>.Default)
        {
        }

        public ObservableSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            _inner = new HashSet<T>(collection, comparer);
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => _inner.Count;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)_inner).IsReadOnly;

        public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Add(T item)
        {
            var changed = _inner.Add(item);
            if (changed)
            {
                RaiseReset();
            }

            return changed;
        }

        void ICollection<T>.Add(T item) => Add(item);

        public void UnionWith(IEnumerable<T> other)
        {
            var count = _inner.Count;
            _inner.UnionWith(other);
            if (count != _inner.Count)
            {
                RaiseReset();
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            var count = _inner.Count;
            _inner.IntersectWith(other);
            if (count != _inner.Count)
            {
                RaiseReset();
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            var count = _inner.Count;
            _inner.ExceptWith(other);
            if (count != _inner.Count)
            {
                RaiseReset();
            }
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            var count = _inner.Count;
            _inner.SymmetricExceptWith(other);
            if (count != _inner.Count)
            {
                RaiseReset();
            }
        }

        public bool IsSubsetOf(IEnumerable<T> other) => _inner.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => _inner.IsSupersetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => _inner.IsProperSupersetOf(other);

        public bool IsProperSubsetOf(IEnumerable<T> other) => _inner.IsProperSubsetOf(other);

        public bool Overlaps(IEnumerable<T> other) => _inner.Overlaps(other);

        public bool SetEquals(IEnumerable<T> other) => _inner.SetEquals(other);

        public void Clear()
        {
            var count = _inner.Count;
            _inner.Clear();
            if (count > 0)
            {
                RaiseReset();
            }
        }

        public bool Contains(T item) => _inner.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            var removed = _inner.Remove(item);
            if (removed)
            {
                RaiseReset();
            }

            return removed;
        }

        private void RaiseReset()
        {
            OnPropertyChanged(CountPropertyChangedEventArgs);
            OnCollectionChanged(ResetEventArgs);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }
    }
}
