namespace Gu.Reactive
{
    using System;
    using System.Collections.Concurrent;

    using Gu.Reactive.Internals;

    internal class Cache<TSource, TResult>
        where TSource : class
        where TResult : class
    {
        private readonly Func<TSource, TResult> selector;
        private readonly ConcurrentDictionary<TSource, Cached> cache = new ConcurrentDictionary<TSource, Cached>(ObjectIdentityComparer<TSource>.Default);

        public Cache(Func<TSource, TResult> selector)
        {
            this.selector = selector;
        }

        public event Action<TResult> OnRemove;

        internal TResult GetOrCreate(TSource key)
        {
            return this.cache.AddOrUpdate(key, this.Create, this.Update).Item;
        }

        internal void Remove(TSource source, TResult mapped)
        {
            Cached cached;
            if (this.cache.TryGetValue(source, out cached))
            {
                if (cached.Decrement() != 0)
                {
                    return;
                }

                if (this.cache.TryRemove(source, out cached))
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

        internal void Clear()
        {
            var handler = this.OnRemove;
            if (handler != null)
            {
                foreach (var cached in this.cache)
                {
                    handler.Invoke(cached.Value.Item);
                }
            }

            this.cache.Clear();
        }

        private Cached Create(TSource source)
        {
            var result = new Cached(this.selector(source));
            return result;
        }

        private Cached Update(TSource source, Cached cached)
        {
            return cached.Increment();
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

            internal Cached Increment()
            {
                this.count++;
                return this;
            }

            internal int Decrement() => this.count--;
        }
    }
}