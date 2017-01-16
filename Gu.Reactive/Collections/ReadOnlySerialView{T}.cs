namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reactive.Disposables;

    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    public sealed class ReadOnlySerialView<T> : ReadonlySerialViewBase<T>, IReadOnlyObservableCollection<T>, IUpdater
    {
        private readonly SerialDisposable refreshSubscription = new SerialDisposable();

        public ReadOnlySerialView()
            : this(null)
        {
        }

        public ReadOnlySerialView(IEnumerable<T> source)
            : base(source, true, true)
        {
            this.refreshSubscription.Disposable = ThrottledRefresher.Create(this, source, TimeSpan.Zero, null, false)
                                                                    .Subscribe(this.Refresh);
        }

        /// <inheritdoc/>
        object IUpdater.IsUpdatingSourceItem => null;

        public new void SetSource(IEnumerable<T> source)
        {
            base.SetSource(source);
            this.refreshSubscription.Disposable = ThrottledRefresher.Create(this, this.Source, TimeSpan.Zero, null, false)
                                                                    .Subscribe(this.Refresh);
        }

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