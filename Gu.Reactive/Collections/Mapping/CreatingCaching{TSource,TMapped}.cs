namespace Gu.Reactive
{
    using System;

    internal class CreatingCaching<TSource, TResult> : IMapper<TSource, TResult>
        where TSource : class
        where TResult : class
    {
        private readonly Cache<TSource, TResult> cache;

        private bool disposed;

        public CreatingCaching(Func<TSource, TResult> selector)
        {
            this.cache = new Cache<TSource, TResult>(selector);
        }

        public bool CanUpdateIndex => false;

        public TResult GetOrCreate(TSource key, int index)
        {
            this.ThrowIfDisposed();
            return this.cache.GetOrCreate(key);
        }

        /// <inheritdoc />
        TResult IMapper<TSource, TResult>.Update(TSource key, TResult oldResult, int index) => oldResult;

        public void Remove(TSource source, TResult mapped)
        {
            this.cache.Remove(source, mapped);
        }

        public IDisposable RefreshTransaction()
        {
            return cache.RefreshTransaction();
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
                this.cache.Clear();
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