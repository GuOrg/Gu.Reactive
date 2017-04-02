namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Concurrency;
    using Gu.Reactive.Internals;

    /// <summary>
    /// An <see cref="ScheduledCollection{T}"/> with support for AddRange and RemoveRange
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    internal class ScheduledCollection<T> : ObservableBatchCollection<T>
    {
        private readonly IScheduler scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledCollection{T}"/> class.
        /// It is empty and has default initial capacity.
        /// </summary>
        /// <param name="scheduler">The scheduler to mutate and notify on.</param>
        public ScheduledCollection(IScheduler scheduler = null)
        {
            this.scheduler = scheduler ?? Scheduler.Immediate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledCollection{T}"/> class.
        /// It contains elements copied from the specified list
        /// </summary>
        /// <param name="collection">The list whose elements are copied to the new list.</param>
        /// <param name="scheduler">The scheduler to mutate and notify on.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the list.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> list is a null reference </exception>
        public ScheduledCollection(IList<T> collection, IScheduler scheduler = null)
            : base(collection)
        {
            this.scheduler = scheduler ?? Scheduler.Immediate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledCollection{T}"/> class.
        /// It contains the elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <param name="scheduler">The scheduler to mutate and notify on.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the collection.
        /// </remarks>
        /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
        public ScheduledCollection(IEnumerable<T> collection, IScheduler scheduler = null)
            : base(collection)
        {
            this.scheduler = scheduler ?? Scheduler.Immediate;
        }

        /// <inheritdoc />
        protected override void AddItems(IEnumerable<T> items)
        {
            this.scheduler.Schedule(() => base.AddItems(items))
                .IgnoreReturnValue();
        }

        /// <inheritdoc />
        protected override void RemoveItems(IEnumerable<T> items)
        {
            this.scheduler.Schedule(() => base.RemoveItems(items))
                .IgnoreReturnValue();
        }

        /// <inheritdoc />
        protected override void ClearItems()
        {
            this.scheduler.Schedule(() => base.ClearItems())
                .IgnoreReturnValue();
        }

        /// <inheritdoc />
        protected override void InsertItem(int index, T item)
        {
            this.scheduler.Schedule(() => base.InsertItem(index, item))
                .IgnoreReturnValue();
        }

        /// <inheritdoc />
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            this.scheduler.Schedule(() => base.MoveItem(oldIndex, newIndex))
                .IgnoreReturnValue();
        }

        /// <inheritdoc />
        protected override void RemoveItem(int index)
        {
            this.scheduler.Schedule(() => base.RemoveItem(index))
                .IgnoreReturnValue();
        }

        /// <inheritdoc />
        protected override void SetItem(int index, T item)
        {
            this.scheduler.Schedule(() => base.SetItem(index, item))
                .IgnoreReturnValue();
        }
    }
}