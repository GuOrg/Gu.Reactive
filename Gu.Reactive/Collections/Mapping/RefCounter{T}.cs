namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gu.Reactive.Internals;

    internal class RefCounter<T>
        where T : class
    {
        private readonly Dictionary<T, int> cache = new Dictionary<T, int>(ObjectIdentityComparer<T>.Default);
        private readonly Dictionary<T, int> transactionCache = new Dictionary<T, int>(ObjectIdentityComparer<T>.Default);
        private readonly object gate = new object();

        private bool isRefreshing;

        public RefCounter()
        {
        }

        public event Action<T> OnRemove;

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

        public void Decrement(T mapped)
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

        public T Increment(T item)
        {
            lock (this.gate)
            {
                var currentCache = this.isRefreshing
                                       ? this.transactionCache
                                       : this.cache;
                if (currentCache.TryGetValue(item, out int count))
                {
                    currentCache[item] = count + 1;
                }
                else
                {
                    currentCache.Add(item, 1);
                }
            }

            return item;
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
            private readonly RefCounter<T> counter;

            private bool disposed;

            public Transaction(RefCounter<T> counter)
            {
                this.counter = counter;
            }

            public void Dispose()
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;
                this.counter.EndRefresh();
            }
        }
    }
}