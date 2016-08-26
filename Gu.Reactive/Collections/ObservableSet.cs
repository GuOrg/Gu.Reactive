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
        private readonly HashSet<T> inner;

        public ObservableSet()
            : this(EqualityComparer<T>.Default)
        {
        }

        public ObservableSet(IEqualityComparer<T> comparer)
        {
            this.inner = new HashSet<T>(comparer);
        }

        public ObservableSet(IEnumerable<T> collection)
            : this(collection, EqualityComparer<T>.Default)
        {
        }

        public ObservableSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            this.inner = new HashSet<T>(collection, comparer);
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => this.inner.Count;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)this.inner).IsReadOnly;

        public IEnumerator<T> GetEnumerator() => this.inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public bool Add(T item)
        {
            var changed = this.inner.Add(item);
            if (changed)
            {
                this.RaiseReset();
            }

            return changed;
        }

        void ICollection<T>.Add(T item) => this.Add(item);

        public void UnionWith(IEnumerable<T> other)
        {
            var count = this.inner.Count;
            this.inner.UnionWith(other);
            if (count != this.inner.Count)
            {
                this.RaiseReset();
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            var count = this.inner.Count;
            this.inner.IntersectWith(other);
            if (count != this.inner.Count)
            {
                this.RaiseReset();
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            var count = this.inner.Count;
            this.inner.ExceptWith(other);
            if (count != this.inner.Count)
            {
                this.RaiseReset();
            }
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            var count = this.inner.Count;
            this.inner.SymmetricExceptWith(other);
            if (count != this.inner.Count)
            {
                this.RaiseReset();
            }
        }

        public bool IsSubsetOf(IEnumerable<T> other) => this.inner.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => this.inner.IsSupersetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => this.inner.IsProperSupersetOf(other);

        public bool IsProperSubsetOf(IEnumerable<T> other) => this.inner.IsProperSubsetOf(other);

        public bool Overlaps(IEnumerable<T> other) => this.inner.Overlaps(other);

        public bool SetEquals(IEnumerable<T> other) => this.inner.SetEquals(other);

        public void Clear()
        {
            var count = this.inner.Count;
            this.inner.Clear();
            if (count > 0)
            {
                this.RaiseReset();
            }
        }

        public bool Contains(T item) => this.inner.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => this.inner.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            var removed = this.inner.Remove(item);
            if (removed)
            {
                this.RaiseReset();
            }

            return removed;
        }

        private void RaiseReset()
        {
            this.OnPropertyChanged(CountPropertyChangedEventArgs);
            this.OnCollectionChanged(ResetEventArgs);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }
    }
}
