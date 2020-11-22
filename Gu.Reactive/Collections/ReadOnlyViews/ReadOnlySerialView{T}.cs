namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reactive.Concurrency;

#pragma warning disable CA1010 // Collections should implement generic interface
    /// <summary>
    /// A view where the source can be updated that notifies about changes.
    /// </summary>
    public class ReadOnlySerialView<T> : ReadOnlySerialViewBase<T>
#pragma warning restore CA1010 // Collections should implement generic interface
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySerialView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        public ReadOnlySerialView(IEnumerable<T> source)
            : this(source, TimeSpan.Zero, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySerialView{T}"/> class.
        /// </summary>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        public ReadOnlySerialView(IScheduler? scheduler = null)
            : this(null, TimeSpan.Zero, scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySerialView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        public ReadOnlySerialView(IEnumerable<T>? source, IScheduler? scheduler = null)
            : this(source, TimeSpan.Zero, scheduler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySerialView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="bufferTime">The time to buffer changes in <paramref name="source"/>.</param>
        /// <param name="scheduler">The scheduler to notify changes on.</param>
        public ReadOnlySerialView(IEnumerable<T>? source, TimeSpan bufferTime, IScheduler? scheduler)
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
        /// <param name="source">The source collection.</param>
        public new void SetSource(IEnumerable<T>? source)
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
