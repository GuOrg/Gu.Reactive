namespace Gu.Reactive.Internals
{
    using System.Collections.Concurrent;

    internal static class IdentityMap
    {
        internal static IdentityMap<TKey, TValue> Borrow<TKey, TValue>()
            where TKey : class
        {
            return Pool<TKey, TValue>.Cache.GetOrCreate(() => new IdentityMap<TKey, TValue>());
        }

        internal static void Return<TKey, TValue>(IdentityMap<TKey, TValue> map)
            where TKey : class
        {
            map.Clear();
            Pool<TKey, TValue>.Cache.Enqueue(map);
        }

        private static class Pool<TKey, TValue>
            where TKey : class
        {
            internal static readonly ConcurrentQueue<IdentityMap<TKey, TValue>> Cache = new ConcurrentQueue<IdentityMap<TKey, TValue>>();
        }
    }
}
