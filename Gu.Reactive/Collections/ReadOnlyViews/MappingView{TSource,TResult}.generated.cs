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
    public partial class MappingView<TSource, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(ObservableCollection<TSource> source, Func<TSource, TResult> selector, TimeSpan bufferTime, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector), bufferTime, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(ObservableCollection<TSource> source, Func<TSource, TResult> selector, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector), TimeSpan.Zero, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(ObservableCollection<TSource> source, Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector, updater), TimeSpan.Zero, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(ObservableCollection<TSource> source, Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater, TimeSpan bufferTime, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector, updater), bufferTime, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(ReadOnlyObservableCollection<TSource> source, Func<TSource, TResult> selector, TimeSpan bufferTime, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector), bufferTime, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(ReadOnlyObservableCollection<TSource> source, Func<TSource, TResult> selector, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector), TimeSpan.Zero, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(ReadOnlyObservableCollection<TSource> source, Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector, updater), TimeSpan.Zero, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(ReadOnlyObservableCollection<TSource> source, Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater, TimeSpan bufferTime, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector, updater), bufferTime, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(IReadOnlyObservableCollection<TSource> source, Func<TSource, TResult> selector, TimeSpan bufferTime, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector), bufferTime, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(IReadOnlyObservableCollection<TSource> source, Func<TSource, TResult> selector, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector), TimeSpan.Zero, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(IReadOnlyObservableCollection<TSource> source, Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector, updater), TimeSpan.Zero, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(IReadOnlyObservableCollection<TSource> source, Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater, TimeSpan bufferTime, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector, updater), bufferTime, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(IObservableCollection<TSource> source, Func<TSource, TResult> selector, TimeSpan bufferTime, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector), bufferTime, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(IObservableCollection<TSource> source, Func<TSource, TResult> selector, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector), TimeSpan.Zero, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(IObservableCollection<TSource> source, Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector, updater), TimeSpan.Zero, scheduler, leaveOpen, triggers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingView{TSource, TResult}"/> class.
        /// </summary>
        public MappingView(IObservableCollection<TSource> source, Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater, TimeSpan bufferTime, IScheduler scheduler = null, bool leaveOpen = false, params IObservable<object>[] triggers)
            : this((IEnumerable<TSource>)source, Mapper.Create(selector, updater), bufferTime, scheduler, leaveOpen, triggers)
        {
        }
    }
}