namespace Gu.Reactive
{
    using System;

    internal class CreatingCaching<TSource, TResult> : IMapper<TSource, TResult>
        where TSource : class
        where TResult : class
    {
        private bool disposed;

        public CreatingCaching(Func<TSource, TResult> selector)
        {
            this.Cache = new Cache<TSource, TResult>(selector);
        }

        public bool CanUpdateIndex => false;

        protected Cache<TSource, TResult> Cache { get; }

        public TResult GetOrCreate(TSource key, int index)
        {
            this.ThrowIfDisposed();
            return this.Cache.GetOrCreate(key);
        }

        /// <inheritdoc />
        TResult IMapper<TSource, TResult>.Update(TSource key, TResult oldResult, int index) => oldResult;

        public void Remove(TSource source, TResult mapped)
        {
            this.Cache.Remove(source, mapped);
        }

        public IDisposable RefreshTransaction()
        {
            return this.Cache.RefreshTransaction();
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
                this.Cache.Clear();
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