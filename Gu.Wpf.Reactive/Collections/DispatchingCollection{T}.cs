namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Threading.Tasks;

    using Gu.Reactive;

    /// <summary>
    /// An <see cref="ObservableBatchCollection{T}"/> that notifies on the dispatcher if needed
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    [Serializable]
    public class DispatchingCollection<T> : ObservableBatchCollection<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchingCollection{T}"/> class.
        /// It is empty and has default initial capacity.
        /// </summary>
        public DispatchingCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchingCollection{T}"/> class.
        /// It contains elements copied from the specified list
        /// </summary>
        /// <param name="collection">The list whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the list.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> list is a null reference </exception>
        public DispatchingCollection(IList<T> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchingCollection{T}"/> class.
        /// It contains the elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the collection.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
        public DispatchingCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <inheritdoc/>
        [field: NonSerialized]
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc/>
        protected override async void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = this.CollectionChanged;
            if (handler != null)
            {
                using (this.BlockReentrancy())
                {
                    await handler.InvokeOnDispatcherAsync(this, e).ConfigureAwait(false);
                }
            }
        }
    }
}
