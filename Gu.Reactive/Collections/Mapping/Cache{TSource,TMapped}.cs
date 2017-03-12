namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Gu.Reactive.Internals;

    internal class Cache<TSource, TResult>
        where TSource : class
        where TResult : class
    {
        private readonly Func<TSource, TResult> selector;
        private readonly Dictionary<TSource, Cached> cache = new Dictionary<TSource, Cached>(ObjectIdentityComparer<TSource>.Default);
        private readonly Dictionary<TSource, Cached> transactionCache = new Dictionary<TSource, Cached>(ObjectIdentityComparer<TSource>.Default);
        private readonly object gate = new object();

        private bool isRefreshing;

        public Cache(Func<TSource, TResult> selector)
        {
            this.selector = selector;
        }

        public event Action<TResult> OnRemove;

        internal TResult GetOrCreate(TSource key)
        {
            lock (this.gate)
            {
                Cached cached;
                if (this.isRefreshing)
                {
                    if (this.transactionCache.TryGetValue(key, out cached))
                    {
                        return cached.Increment();
                    }

                    cached = this.cache.TryGetValue(key, out cached)
                        ? new Cached(cached.Item)
                        : new Cached(this.selector(key));

                    this.transactionCache.Add(key, cached);
                    return cached.Item;
                }

                if (this.cache.TryGetValue(key, out cached))
                {
                    return cached.Increment();
                }

                cached = new Cached(this.selector(key));
                this.cache.Add(key, cached);
                return cached.Item;
            }
        }

        internal void Remove(TSource source, TResult mapped)
        {
            lock (this.gate)
            {
                Cached cached;
                if (this.cache.TryGetValue(source, out cached))
                {
                    if (cached.Decrement() > 0)
                    {
                        return;
                    }

                    var handler = this.OnRemove;
                    if (handler != null)
                    {
                        if (this.cache.Remove(source))
                        {
                            // one mapped can be created from many sources.
                            foreach (var kvp in this.cache)
                            {
                                if (ReferenceEquals(kvp.Value.Item, mapped))
                                {
                                    return;
                                }
                            }

                            this.OnRemove?.Invoke(mapped);
                        }
                    }
                }
            }
        }

        internal void Clear()
        {
            lock (this.gate)
            {
                var handler = this.OnRemove;
                if (handler != null)
                {
                    foreach (var cached in this.cache.Values)
                    {
                        handler.Invoke(cached.Item);
                    }
                }

                this.cache.Clear();
            }
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
                var set = SetPool.Borrow<TResult>();
                foreach (var value in this.transactionCache.Values)
                {
                    set.Add(value.Item);
                }

                foreach (var value in this.cache.Values)
                {
                    if (!set.Contains(value.Item))
                    {
                        handler.Invoke(value.Item);
                    }
                }

                SetPool.Return(set);
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
            private readonly Cache<TSource, TResult> cache;

            private bool disposed;

            public Transaction(Cache<TSource, TResult> cache)
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

        private class Cached
        {
            internal readonly TResult Item;
            private int count;

            public Cached(TResult item)
            {
                this.Item = item;
                this.count = 1;
            }

            internal TResult Increment()
            {
                this.count++;
                return this.Item;
            }

            internal int Decrement() => --this.count;
        }
    }
}