namespace Gu.Reactive
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using Gu.Reactive.Internals;

    internal class CreatingRemoving<TSource, TResult> : IMapper<TSource, TResult>
        where TSource : class 
        where TResult : class
    {
        private readonly Func<TSource, TResult> selector;
        private readonly ConcurrentDictionary<TSource, TResult> cache = new ConcurrentDictionary<TSource, TResult>(ObjectIdentityComparer<TSource>.Default);
        private readonly Action<TResult> onRemove;

        private bool disposed;

        internal CreatingRemoving(Func<TSource, TResult> selector, Action<TResult> onRemove)
        {
            this.selector = selector;
            this.onRemove = onRemove;
        }

        public bool CanUpdateIndex => false;

        public TResult GetOrCreateValue(TSource key, int index)
        {
            this.ThrowIfDisposed();
            return this.cache.GetOrAdd(key, this.selector);
        }

        /// <inheritdoc />
        TResult IMapper<TSource, TResult>.UpdateIndex(TSource key, TResult oldResult, int index) => oldResult;

        /// <inheritdoc/>
        public virtual void Refresh(IEnumerable<TSource> source, IReadOnlyList<TResult> mapped, NotifyCollectionChangedEventArgs e)
        {
            this.ThrowIfDisposed();
            var set = SetPool.Borrow<TSource>();
            set.UnionWith(this.cache.Select(x => x.Key));
            set.ExceptWith(source);
            foreach (var item in set)
            {
                TResult removed;
                if (this.cache.TryRemove(item, out removed))
                {
                    this.onRemove(removed);
                }
            }

            SetPool.Return(set);
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
                foreach (var kvp in this.cache)
                {
                    this.onRemove(kvp.Value);
                }

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