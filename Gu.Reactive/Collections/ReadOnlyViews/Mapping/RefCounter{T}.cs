namespace Gu.Reactive
{
    using System;
    using System.Threading;

    internal class RefCounter<T>
        where T : class
    {
        private readonly InstanceMap<T, int> cache = new InstanceMap<T, int>();
        private readonly InstanceMap<T, int> transactionCache = new InstanceMap<T, int>();

        private bool isRefreshing;

        internal event Action<T> OnRemove;

        private object Gate => this.cache.Gate;

        internal void Clear()
        {
            lock (this.Gate)
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

        internal void Decrement(T mapped)
        {
            lock (this.Gate)
            {
                this.cache[mapped]--;
                if (this.cache[mapped] <= 0)
                {
                    this.OnRemove?.Invoke(mapped);
                    this.cache.Remove(mapped);
                }
            }
        }

        internal T Increment(T item)
        {
            lock (this.Gate)
            {
                var currentCache = this.isRefreshing
                                       ? this.transactionCache
                                       : this.cache;
                if (currentCache.TryGetValue(item, out var count))
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
            Monitor.Enter(this.Gate);
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
            Monitor.Exit(this.Gate);
        }

        private sealed class Transaction : IDisposable
        {
            private readonly RefCounter<T> counter;

            private bool disposed;

            internal Transaction(RefCounter<T> counter)
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
