namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Gu.Reactive.Internals;

    internal class CreatingRemoving<TSource, TResult> : IMapper<TSource, TResult>
        where TSource : struct
        where TResult : class
    {
        private readonly InstanceTracker tracker;
        private readonly Action<TResult> onRemove;
        private bool disposed;

        internal CreatingRemoving(Func<TSource, TResult> selector, Action<TResult> onRemove)
        {
            this.tracker = new InstanceTracker(selector);
            this.onRemove = onRemove;
            this.tracker.OnRemove += onRemove;
        }

        public bool CanUpdateIndex => false;

        public TResult GetOrCreate(TSource key, int index)
        {
            this.ThrowIfDisposed();
            return this.tracker.GetOrCreate(key);
        }

        /// <inheritdoc />
        TResult IMapper<TSource, TResult>.Update(TSource key, TResult oldResult, int index) => oldResult;

        public void Remove(TSource source, TResult mapped)
        {
            this.tracker.Remove(mapped);
        }

        public IDisposable RefreshTransaction()
        {
            return this.tracker.RefreshTransaction();
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
                this.tracker.Clear();
                this.tracker.OnRemove -= this.onRemove;
            }
        }

        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private class InstanceTracker
        {
            private readonly Func<TSource, TResult> selector;
            private readonly Dictionary<TResult, int> cache = new Dictionary<TResult, int>(ObjectIdentityComparer<TResult>.Default);
            private readonly Dictionary<TResult, int> transactionCache = new Dictionary<TResult, int>(ObjectIdentityComparer<TResult>.Default);
            private readonly object gate = new object();

            private bool isRefreshing;

            public InstanceTracker(Func<TSource, TResult> selector)
            {
                this.selector = selector;
            }

            public event Action<TResult> OnRemove;

            public void Clear()
            {
                lock (this.gate)
                {
                    var handler = this.OnRemove;
                    if (handler != null)
                    {
                        foreach (var key in this.cache.Keys)
                        {
                            handler.Invoke(key);
                        }
                    }

                    this.cache.Clear();
                }
            }

            public void Remove(TResult mapped)
            {
                lock (this.gate)
                {
                    this.cache[mapped]--;
                    if (this.cache[mapped] <= 0)
                    {
                        this.OnRemove?.Invoke(mapped);
                        this.cache.Remove(mapped);
                    }
                }
            }

            public TResult GetOrCreate(TSource key)
            {
                var value = this.selector(key);
                lock (this.gate)
                {
                    var currentCache = this.isRefreshing
                        ? this.transactionCache
                        : this.cache;
                    if (currentCache.TryGetValue(value, out int count))
                    {
                        currentCache[value] = count + 1;
                    }
                    else
                    {
                        currentCache.Add(value, 1);
                    }
                }

                return value;
            }

            internal IDisposable RefreshTransaction()
            {
                Monitor.Enter(this.gate);
                this.isRefreshing = true;
                return new Transaction(this);
            }

            private void EndRefresh()
            {
                var handler = this.OnRemove;
                if (handler != null)
                {
                    foreach (var key in this.cache.Keys)
                    {
                        if (!this.transactionCache.ContainsKey(key))
                        {
                            handler.Invoke(key);
                        }
                    }
                }

                this.cache.Clear();
                foreach (var kvp in this.transactionCache)
                {
                    this.cache.Add(kvp.Key, kvp.Value);
                }

                this.transactionCache.Clear();
                this.isRefreshing = false;
                Monitor.Exit(this.gate);
            }

            private sealed class Transaction : IDisposable
            {
                private readonly InstanceTracker cache;

                private bool disposed;

                public Transaction(InstanceTracker cache)
                {
                    this.cache = cache;
                }

                public void Dispose()
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    this.disposed = true;
                    this.cache.EndRefresh();
                }
            }
        }
    }
}