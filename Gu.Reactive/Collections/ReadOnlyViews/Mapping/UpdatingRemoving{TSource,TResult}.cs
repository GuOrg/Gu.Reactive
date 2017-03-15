namespace Gu.Reactive
{
    using System;

    internal class UpdatingRemoving<TSource, TResult> : IMapper<TSource, TResult>
        where TResult : class
    {
        private readonly RefCounter<TResult> refCounter = new RefCounter<TResult>();
        private readonly Func<TSource, int, TResult> selector;
        private readonly Func<TResult, int, TResult> updater;
        private readonly Action<TResult> onRemove;

        private bool disposed;

        internal UpdatingRemoving(Func<TSource, int, TResult> selector, Func<TResult, int, TResult> updater, Action<TResult> onRemove)
        {
            this.selector = selector;
            this.updater = updater;
            this.onRemove = onRemove;
            this.refCounter.OnRemove += onRemove;
        }

        public bool CanUpdateIndex => true;

        public TResult GetOrCreate(TSource key, int index)
        {
            this.ThrowIfDisposed();
            return this.refCounter.Increment(this.selector(key, index));
        }

        /// <inheritdoc />
        TResult IMapper<TSource, TResult>.Update(TSource key, TResult old, int index)
        {
            this.ThrowIfDisposed();
            var updated = this.updater(old, index);
            if (ReferenceEquals(old, updated))
            {
                return updated;
            }

            this.refCounter.Decrement(old);
            this.refCounter.Increment(updated);
            return updated;
        }

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