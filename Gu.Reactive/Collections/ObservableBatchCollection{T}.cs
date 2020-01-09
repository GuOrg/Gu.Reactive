// ReSharper disable StaticMemberInGenericType
namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// An <see cref="ObservableCollection{T}"/> with support for AddRange and RemoveRange.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    [Serializable]
    public class ObservableBatchCollection<T> : ObservableCollection<T>
    {
        private static readonly PropertyChangedEventArgs CountPropertyChangedEventArgs = CachedEventArgs.CountPropertyChanged;
        private static readonly PropertyChangedEventArgs IndexerPropertyChangedEventArgs = CachedEventArgs.IndexerPropertyChanged;
        private static readonly NotifyCollectionChangedEventArgs NotifyCollectionResetEventArgs = CachedEventArgs.NotifyCollectionReset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableBatchCollection{T}"/> class.
        /// It is empty and has default initial capacity.
        /// </summary>
        public ObservableBatchCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableBatchCollection{T}"/> class.
        /// It contains elements copied from the specified list.
        /// </summary>
        /// <param name="collection">The list whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the list.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> list is a null reference. </exception>
        public ObservableBatchCollection(IList<T> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableBatchCollection{T}"/> class.
        /// It contains the elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the collection.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> collection is a null reference. </exception>
        public ObservableBatchCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Add a range of elements. If the count is greater than 1 one reset event is raised when done.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public void AddRange(IEnumerable<T> items)
        {
            this.AddItems(items);
        }

        /// <summary>
        /// Remove a range of elements. If the count is greater than 1 one reset event is raised when done.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public void RemoveRange(IEnumerable<T> items)
        {
            this.RemoveItems(items);
        }

        /// <summary>Removes all the elements that match the conditions defined by the specified predicate.</summary>
        /// <param name="predicate">The <see cref="System.Predicate{T}" /> delegate that defines the conditions of the elements to remove.</param>
        /// <returns>The number of elements removed from the <see cref="ObservableBatchCollection{T}" /> .</returns>
        public int RemoveAll(Func<T, bool> predicate)
        {
            this.CheckReentrancy();
            var removed = 0;
            for (var i = this.Items.Count - 1; i >= 0; i--)
            {
                if (predicate(this.Items[i]))
                {
                    removed++;
                    this.Items.RemoveAt(i);
                }
            }

            if (removed == 0)
            {
                return 0;
            }

            this.RaiseReset();
            return removed;
        }

        /// <summary>
        /// 1. Clear the collection.
        /// 2. AddRange <paramref name="items"/>
        /// 3. Notify reset once.
        /// </summary>
        /// <param name="items">The new contents of the collection.</param>
        public void ResetTo(IEnumerable<T> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            this.CheckReentrancy();
            this.Items.Clear();
            foreach (var item in items)
            {
                this.Items.Add(item);
            }

            this.RaiseReset();
        }

        /// <summary>
        /// Add a range of elements. If the count is greater than 1 one reset event is raised when done.
        /// </summary>
        /// <param name="items">The items to add.</param>
        protected virtual void AddItems(IEnumerable<T> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            this.CheckReentrancy();
            var before = this.Items.Count;
            using var e = items.GetEnumerator();
            while (e.MoveNext())
            {
                this.Items.Add(e.Current);
            }

            if (this.Items.Count == before + 1)
            {
                this.OnPropertyChanged(CountPropertyChangedEventArgs);
                this.OnPropertyChanged(IndexerPropertyChangedEventArgs);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this.Items[this.Items.Count - 1], this.Items.Count - 1));
            }
            else if (this.Items.Count != before)
            {
                this.RaiseReset();
            }
        }

        /// <summary>
        /// Remove a range of elements. If the count is greater than 1 one reset event is raised when done.
        /// </summary>
        /// <param name="items">The items to add.</param>
        protected virtual void RemoveItems(IEnumerable<T> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            this.CheckReentrancy();
            var before = this.Items.Count;
            //// using KeyValuePair here, not dragging in reference to value tuple just for this.
            KeyValuePair<int, T>? first = null;
            using var e = items.GetEnumerator();
            while (e.MoveNext())
            {
                if (first is null &&
                    this.Items.IndexOf(e.Current) is { } i &&
                    i >= 0)
                {
                    first = new KeyValuePair<int, T>(i, e.Current);
                }

                while (this.Items.Remove(e.Current))
                {
                }
            }

            if (this.Items.Count == before - 1 &&
                first is { Key: { } index, Value: var removed })
            {
                this.OnPropertyChanged(CountPropertyChangedEventArgs);
                this.OnPropertyChanged(IndexerPropertyChangedEventArgs);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, index));
            }
            else if (this.Items.Count != before)
            {
                this.RaiseReset();
            }
        }

#pragma warning disable CA1030 // Use events where appropriate
        /// <summary>
        /// Raise reset events, Count, Item[] then CollectionReset.
        /// </summary>
        protected virtual void RaiseReset()
#pragma warning restore CA1030 // Use events where appropriate
        {
            this.OnPropertyChanged(CountPropertyChangedEventArgs);
            this.OnPropertyChanged(IndexerPropertyChangedEventArgs);
            this.OnCollectionChanged(NotifyCollectionResetEventArgs);
        }
    }
}
