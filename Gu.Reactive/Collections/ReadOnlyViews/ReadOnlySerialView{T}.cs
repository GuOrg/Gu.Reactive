namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reactive.Concurrency;

    /// <summary>
    /// A view where the source can be updated that notifies about changes.
    /// </summary>
    public class ReadOnlySerialView<T> : ReadOnlySerialViewBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySerialView{T}"/> class.
        /// </summary>
        public ReadOnlySerialView(IEnumerable<T> source)
            : this(source, TimeSpan.Zero, null)
        {
        }

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
            : base(source, bufferTime, scheduler, leaveOpen: true)
        {
        }

        /// <inheritdoc/>
        public override void Refresh()
        {
            using (this.Chunk.ClearTransaction())
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
    }
}