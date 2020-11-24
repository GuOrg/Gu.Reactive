// ReSharper disable StaticMemberInGenericType
namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;

    /// <summary>
    /// A set that notifies about changes.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {this.Count}")]
    [Serializable]
    public class ObservableSet<T> : IObservableSet<T>, IReadOnlyCollection<T>, IReadonlySet<T>
    {
        private static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(Count));
        private static readonly NotifyCollectionChangedEventArgs ResetEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        private readonly HashSet<T> inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableSet{T}"/> class.
        /// Uses <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        public ObservableSet()
            : this(EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableSet{T}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/>.</param>
        public ObservableSet(IEqualityComparer<T> comparer)
        {
            this.inner = new HashSet<T>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableSet{T}"/> class.
        /// Uses <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IEnumerable{T}"/>.</param>
        public ObservableSet(IEnumerable<T> collection)
            : this(collection, EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableSet{T}"/> class.
        /// </summary>
        /// <param name="collection">The <see cref="IEnumerable{T}"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/>.</param>
        public ObservableSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            this.inner = new HashSet<T>(collection, comparer);
        }

        /// <inheritdoc/>
        [field: NonSerialized]
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <inheritdoc cref="IReadOnlyCollection{T}" />
        public int Count => this.inner.Count;

#pragma warning disable CA1033 // Interface methods should be callable by child types
        /// <inheritdoc/>
        bool ICollection<T>.IsReadOnly => ((ICollection<T>)this.inner).IsReadOnly;
#pragma warning restore CA1033 // Interface methods should be callable by child types

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => this.inner.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc/>
        public bool Add(T item)
        {
            var changed = this.inner.Add(item);
            if (changed)
            {
                this.RaiseReset();
            }

            return changed;
        }

        /// <inheritdoc/>
        void ICollection<T>.Add(T item) => this.Add(item);

        /// <inheritdoc/>
        public void UnionWith(IEnumerable<T> other)
        {
            var count = this.inner.Count;
            this.inner.UnionWith(other);
            if (count != this.inner.Count)
            {
                this.RaiseReset();
            }
        }

        /// <inheritdoc/>
        public void IntersectWith(IEnumerable<T> other)
        {
            var count = this.inner.Count;
            this.inner.IntersectWith(other);
            if (count != this.inner.Count)
            {
                this.RaiseReset();
            }
        }

        /// <inheritdoc/>
        public void ExceptWith(IEnumerable<T> other)
        {
            var count = this.inner.Count;
            this.inner.ExceptWith(other);
            if (count != this.inner.Count)
            {
                this.RaiseReset();
            }
        }

        /// <inheritdoc/>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            var count = this.inner.Count;
            this.inner.SymmetricExceptWith(other);
            if (count != this.inner.Count)
            {
                this.RaiseReset();
            }
        }

        /// <inheritdoc cref="IReadonlySet{T}" />
        public bool IsSubsetOf(IEnumerable<T> other) => this.inner.IsSubsetOf(other);

        /// <inheritdoc cref="IReadonlySet{T}" />
        public bool IsSupersetOf(IEnumerable<T> other) => this.inner.IsSupersetOf(other);

        /// <inheritdoc cref="IReadonlySet{T}" />
        public bool IsProperSupersetOf(IEnumerable<T> other) => this.inner.IsProperSupersetOf(other);

        /// <inheritdoc cref="IReadonlySet{T}" />
        public bool IsProperSubsetOf(IEnumerable<T> other) => this.inner.IsProperSubsetOf(other);

        /// <inheritdoc cref="IReadonlySet{T}" />
        public bool Overlaps(IEnumerable<T> other) => this.inner.Overlaps(other);

        /// <inheritdoc cref="IReadonlySet{T}" />
        public bool SetEquals(IEnumerable<T> other) => this.inner.SetEquals(other);

        /// <inheritdoc/>
        public void Clear()
        {
            var count = this.inner.Count;
            this.inner.Clear();
            if (count > 0)
            {
                this.RaiseReset();
            }
        }

        /// <inheritdoc/>
        public bool Contains(T item) => this.inner.Contains(item);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => this.inner.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            var removed = this.inner.Remove(item);
            if (removed)
            {
                this.RaiseReset();
            }

            return removed;
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="ObservableSet{T}"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/>.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raise CollectionChanged event to any listeners.
        /// Properties/methods modifying this <see cref="ObservableSet{T}"/> will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/>.</param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        private void RaiseReset()
        {
            this.OnPropertyChanged(CountPropertyChangedEventArgs);
            this.OnCollectionChanged(ResetEventArgs);
        }
    }
}
