namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reactive.Disposables;

    /// <summary>
    /// A view where the source can be updated that notifies about changes.
    /// </summary>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public sealed class ReadOnlySerialView<T> : ReadonlySerialViewBase<T>, IReadOnlyObservableCollection<T>, IUpdater
    {
        private readonly SerialDisposable refreshSubscription = new SerialDisposable();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySerialView{T}"/> class.
        /// </summary>
        public ReadOnlySerialView()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlySerialView{T}"/> class.
        /// </summary>
        public ReadOnlySerialView(IEnumerable<T> source)
            : base(source, true, true)
        {
            this.refreshSubscription.Disposable = ThrottledRefresher.Create(this, source, TimeSpan.Zero, null, false)
                                                                    .Subscribe(this.Refresh);
        }

        /// <inheritdoc/>
        object IUpdater.CurrentlyUpdatingSourceItem => null;

        /// <summary>
        /// Update the source collection and notify about changes.
        /// </summary>
        public new void SetSource(IEnumerable<T> source)
        {
            base.SetSource(source);
            this.refreshSubscription.Disposable = ThrottledRefresher.Create(this, this.Source, TimeSpan.Zero, null, false)
                                                                    .Subscribe(this.Refresh);
        }

        /// <summary>
        /// Set Source to empty and notify about changes.
        /// </summary>
        public new void ClearSource()
        {
            base.ClearSource();
            this.refreshSubscription.Disposable = null;
        }

        /// <summary>
        /// Disposes of a <see cref="ReadOnlySerialView{T}"/>.
        /// </summary>
        /// <remarks>
        /// Called from Dispose() with disposing=true.
        /// Guidelines:
        /// 1. We may be called more than once: do nothing after the first call.
        /// 2. Avoid throwing exceptions if disposing is false, i.e. if we're being finalized.
        /// </remarks>
        /// <param name="disposing">True if called from Dispose(), false if called from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.refreshSubscription.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}