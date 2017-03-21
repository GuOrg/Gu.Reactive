namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    /// <summary>
    /// A view where the source can be updated that notifies about changes.
    /// </summary>
    public class ReadOnlySerialView<T> : ReadonlySerialViewBase<T, T>, IReadOnlyObservableCollection<T>
    {
        private readonly IDisposable refreshSubscription;
        private readonly Chunk<NotifyCollectionChangedEventArgs> chunk;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySerialView{T}"/> class.
        /// </summary>
        public ReadOnlySerialView(IScheduler scheduler = null)
            : this(null, TimeSpan.Zero, scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySerialView{T}"/> class.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public ReadOnlySerialView(IEnumerable<T> source, IScheduler scheduler = null)
            : this(source, TimeSpan.Zero, scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySerialView{T}"/> class.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public ReadOnlySerialView(IEnumerable<T> source, TimeSpan bufferTime, IScheduler scheduler)
            : base(source, s => s, true, true)
        {
            this.chunk = new Chunk<NotifyCollectionChangedEventArgs>(bufferTime, scheduler ?? DefaultScheduler.Instance);
            this.refreshSubscription = this.ObserveValue(x => x.Source)
                                           .Select(x => x.GetValueOrDefault()
                                                         .ObserveCollectionChangedSlimOrDefault(true)
                                                         .Slide(this.chunk))
                                           .Switch()
                                           .ObserveOn(scheduler ?? ImmediateScheduler.Instance)
                                           .Subscribe(this.Refresh);
        }

        /// <inheritdoc/>
        public override void Refresh()
        {
            using (this.chunk.ClearTransaction())
            {
                base.Refresh();
            }
        }

        /// <summary>
        /// Update the source collection and notify about changes.
        /// </summary>
        public new void SetSource(IEnumerable<T> source)
        {
            // new to change it to public.
            base.SetSource(source);
        }

        /// <summary>
        /// Set Source to empty and notify about changes.
        /// </summary>
        public new void ClearSource()
        {
            base.ClearSource();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.refreshSubscription.Dispose();
                this.chunk.ClearItems();
            }

            base.Dispose(disposing);
        }

        private void Refresh(Chunk<NotifyCollectionChangedEventArgs> changes)
        {
            using (changes.ClearTransaction())
            {
                if (changes.Count > 0)
                {
                    base.Refresh(changes);
                }
            }
        }
    }
}