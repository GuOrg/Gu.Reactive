namespace Gu.Reactive
{
    using System;

    internal class CreatingRemoving<TSource, TResult> : IMapper<TSource, TResult>
        where TSource : struct
        where TResult : class
    {
        private readonly RefCounter<TResult> refCounter;
        private readonly Func<TSource, TResult> selector;
        private readonly Action<TResult> onRemove;
        private bool disposed;

        internal CreatingRemoving(Func<TSource, TResult> selector, Action<TResult> onRemove)
        {
            this.refCounter = new RefCounter<TResult>();
            this.selector = selector;
            this.onRemove = onRemove;
            this.refCounter.OnRemove += onRemove;
        }

        public bool CanUpdateIndex => false;

        public TResult GetOrCreate(TSource key, int index)
        {
            this.ThrowIfDisposed();
            return this.refCounter.Increment(this.selector(key));
        }

        /// <inheritdoc />
        TResult IMapper<TSource, TResult>.Update(TSource key, TResult oldResult, int index) => oldResult;

        public void Remove(TSource source, TResult mapped)
        {
            this.refCounter.Decrement(mapped);
        }

        public IDisposable RefreshTransaction()
        {
            return this.refCounter.RefreshTransaction();
        }

        /// <inheritdoc />
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
                this.refCounter.Clear();
                this.refCounter.OnRemove -= this.onRemove;
            }
        }

        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}