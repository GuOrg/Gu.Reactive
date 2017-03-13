namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Gu.Reactive.Internals;

    internal class CreatingCaching<TSource, TResult> : IMapper<TSource, TResult>
        where TSource : class
        where TResult : class
    {
        private bool disposed;

        public CreatingCaching(Func<TSource, TResult> selector)
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
            private readonly Dictionary<TSource, Cached> cache = new Dictionary<TSource, Cached>(ObjectIdentityComparer<TSource>.Default);
            private readonly Dictionary<TSource, Cached> transactionCache = new Dictionary<TSource, Cached>(ObjectIdentityComparer<TSource>.Default);

            private bool isRefreshing;

            public InstanceCache(Func<TSource, TResult> selector)
            {
                this.selector = selector;
            }

            public event Action<TResult> OnRemove;

            private object Gate => ((ICollection)this.cache).SyncRoot;

            internal TResult GetOrCreate(TSource key)
            {
                lock (this.Gate)
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
                lock (this.Gate)
                {
                    Cached cached;
                    if (this.cache.TryGetValue(source, out cached))
                    {
                        if (cached.Decrement() > 0)
                        {
                            return;
                        }

                        if (this.cache.Remove(source))
                        {
                            var handler = this.OnRemove;
                            if (handler != null)
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
                lock (this.Gate)
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
                Monitor.Enter(this.Gate);
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
                Monitor.Exit(this.Gate);
            }

            private sealed class Transaction : IDisposable
            {
                private readonly InstanceCache cache;

                private bool disposed;

                public Transaction(InstanceCache cache)
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
}