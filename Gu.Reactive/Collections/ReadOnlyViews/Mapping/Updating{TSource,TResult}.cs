namespace Gu.Reactive
{
    using System;
    using System.Reactive.Disposables;

    internal class Updating<TSource, TResult> : IMapper<TSource, TResult>
    {
        private readonly Func<TSource, int, TResult> selector;
        private readonly Func<TResult, int, TResult> updater;
        private bool disposed;

        internal Updating(Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater)
        {
            this.selector = selector;
            this.updater = updater ?? Id;
        }

        protected Updating(Func<TSource, int, TResult> selector)
            : this(selector, null)
        {
        }

        public virtual bool CanUpdateIndex => true;

        public virtual TResult GetOrCreate(TSource key, int index) => this.selector(key, index);

        public virtual TResult Update(TSource key, TResult oldResult, int index) => this.updater(oldResult, index);

        void IMapper<TSource, TResult>.Remove(TSource source, TResult mapped)
        {
        }

        IDisposable IMapper<TSource, TResult>.RefreshTransaction() => Disposable.Empty;

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
            }
        }

        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private static TResult Id(TResult olResult, int index) => olResult;
    }
}