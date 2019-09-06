namespace Gu.Reactive
{
    using System;
    using System.Threading;

    internal class CreatingCaching<TSource, TResult> : IMapper<TSource, TResult>
        where TSource : class
        where TResult : class
    {
        private bool disposed;

        internal CreatingCaching(Func<TSource, TResult> selector)
        {
            this.Cache = new InstanceCache(selector);
        }

        public bool CanUpdateIndex => false;

        protected InstanceCache Cache { get; }

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
            GC.SuppressFinalize(this);
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

        protected class InstanceCache
        {
            private readonly Func<TSource, TResult> selector;
            private readonly InstanceMap<TSource, TResult> cache = new InstanceMap<TSource, TResult>();
            private readonly RefCounter<TResult> resultCounter = new RefCounter<TResult>();
            private readonly RefCounter<TSource> sourceCounter = new RefCounter<TSource>();

            internal InstanceCache(Func<TSource, TResult> selector)
            {
                this.selector = selector;
                this.sourceCounter.OnRemove += x => this.cache.Remove(x);
            }

            internal event Action<TResult> OnRemove
            {
                add => this.resultCounter.OnRemove += value;
                remove => this.resultCounter.OnRemove -= value;
            }

            private object Gate => this.cache.Gate;

            internal TResult GetOrCreate(TSource key)
            {
                lock (this.Gate)
                {
                    if (!this.cache.TryGetValue(key, out TResult result))
                    {
                        result = this.selector(key);
                        this.cache.Add(key, result);
                    }

                    this.sourceCounter.Increment(key);
                    this.resultCounter.Increment(result);
                    return result;
                }
            }

            internal void Remove(TSource source, TResult mapped)
            {
                lock (this.Gate)
                {
                    this.sourceCounter.Decrement(source);
                    this.resultCounter.Decrement(mapped);
                }
            }

            internal void Clear()
            {
                lock (this.Gate)
                {
                    this.cache.Clear();
                    this.resultCounter.Clear();
                    this.sourceCounter.Clear();
                }
            }

            internal IDisposable RefreshTransaction()
            {
                Monitor.Enter(this.Gate);
                return new Transaction(this);
            }

            private void EndRefresh()
            {
                Monitor.Exit(this.Gate);
            }

            private sealed class Transaction : IDisposable
            {
                private readonly InstanceCache cache;
                private readonly IDisposable resultTransaction;
                private readonly IDisposable sourceTransaction;

                private bool disposed;

                internal Transaction(InstanceCache cache)
                {
                    this.cache = cache;
                    this.resultTransaction = cache.resultCounter.RefreshTransaction();
                    this.sourceTransaction = cache.sourceCounter.RefreshTransaction();
                }

                public void Dispose()
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    this.disposed = true;
                    this.resultTransaction?.Dispose();
                    this.sourceTransaction?.Dispose();
                    this.cache.EndRefresh();
                }
            }
        }
    }
}
