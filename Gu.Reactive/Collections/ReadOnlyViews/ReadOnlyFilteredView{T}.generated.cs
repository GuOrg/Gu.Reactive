#pragma warning disable SA1619 // Generic type parameters must be documented partial class
namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;

    /// <summary>
    /// Generated constructors
    /// </summary>
    public partial class ReadOnlyFilteredView<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer source changes before updating the view.</param>
        /// <param name="scheduler">The scheduler to perform the filtering and notification on.</param>
        /// <param name="triggers">Additional triggers for when to filter.</param>
        public ReadOnlyFilteredView(ObservableCollection<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, filter, bufferTime, scheduler, false, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer source changes before updating the view.</param>
        /// <param name="scheduler">The scheduler to perform the filtering and notification on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        /// <param name="triggers">Additional triggers for when to filter.</param>
        public ReadOnlyFilteredView(ObservableCollection<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, bool leaveOpen, params IObservable<object>[] triggers)
            : this(source, filter, bufferTime, scheduler, leaveOpen, (IEnumerable<IObservable<object>>)triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer source changes before updating the view.</param>
        /// <param name="scheduler">The scheduler to perform the filtering and notification on.</param>
        /// <param name="triggers">Additional triggers for when to filter.</param>
        public ReadOnlyFilteredView(ReadOnlyObservableCollection<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, filter, bufferTime, scheduler, false, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer source changes before updating the view.</param>
        /// <param name="scheduler">The scheduler to perform the filtering and notification on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        /// <param name="triggers">Additional triggers for when to filter.</param>
        public ReadOnlyFilteredView(ReadOnlyObservableCollection<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, bool leaveOpen, params IObservable<object>[] triggers)
            : this(source, filter, bufferTime, scheduler, leaveOpen, (IEnumerable<IObservable<object>>)triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer source changes before updating the view.</param>
        /// <param name="scheduler">The scheduler to perform the filtering and notification on.</param>
        /// <param name="triggers">Additional triggers for when to filter.</param>
        public ReadOnlyFilteredView(IReadOnlyObservableCollection<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, filter, bufferTime, scheduler, false, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer source changes before updating the view.</param>
        /// <param name="scheduler">The scheduler to perform the filtering and notification on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        /// <param name="triggers">Additional triggers for when to filter.</param>
        public ReadOnlyFilteredView(IReadOnlyObservableCollection<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, bool leaveOpen, params IObservable<object>[] triggers)
            : this(source, filter, bufferTime, scheduler, leaveOpen, (IEnumerable<IObservable<object>>)triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer source changes before updating the view.</param>
        /// <param name="scheduler">The scheduler to perform the filtering and notification on.</param>
        /// <param name="triggers">Additional triggers for when to filter.</param>
        public ReadOnlyFilteredView(IObservableCollection<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, params IObservable<object>[] triggers)
            : this(source, filter, bufferTime, scheduler, false, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyFilteredView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="filter">The predicate to filter by.</param>
        /// <param name="bufferTime">The time to buffer source changes before updating the view.</param>
        /// <param name="scheduler">The scheduler to perform the filtering and notification on.</param>
        /// <param name="leaveOpen">True means that the <paramref name="source"/> is not disposed when this instance is diposed.</param>
        /// <param name="triggers">Additional triggers for when to filter.</param>
        public ReadOnlyFilteredView(IObservableCollection<T> source, Func<T, bool> filter, TimeSpan bufferTime, IScheduler scheduler, bool leaveOpen, params IObservable<object>[] triggers)
            : this(source, filter, bufferTime, scheduler, leaveOpen, (IEnumerable<IObservable<object>>)triggers)
        {
        }
    }
}