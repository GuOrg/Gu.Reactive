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
        private readonly SerialDisposable _refreshSubscription = new SerialDisposable();

        public ReadOnlySerialView()
            : this(null)
        {
        }

        public ReadOnlySerialView(IEnumerable<T> source)
            : base(source, true, true)
        {
            _refreshSubscription.Disposable = ThrottledRefresher.Create(this, source, TimeSpan.Zero, null, false)
                                                                .Subscribe(Refresh);
        }

        object IUpdater.IsUpdatingSourceItem => null;

        public new void SetSource(IEnumerable<T> source)
        {
            base.SetSource(source);
            _refreshSubscription.Disposable = ThrottledRefresher.Create(this, Source, TimeSpan.Zero, null, false)
                                                                .Subscribe(Refresh);
        }

        public new void ClearSource()
        {
            base.ClearSource();
            _refreshSubscription.Disposable = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _refreshSubscription.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}