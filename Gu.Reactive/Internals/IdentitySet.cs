namespace Gu.Reactive.Internals
{
    using System.Collections.Concurrent;

    internal static class IdentitySet
    {
        internal static IdentitySet<T> Borrow<T>()
            where T : class
        {
            return Pool<T>.Cache.GetOrCreate(() => new IdentitySet<T>());
        }

        internal static void Return<T>(IdentitySet<T> set)
            where T : class
        {
            set.Clear();
            Pool<T>.Cache.Enqueue(set);
        }

        private static class Pool<T>
            where T : class
        {
            internal static readonly ConcurrentQueue<IdentitySet<T>> Cache = new ConcurrentQueue<IdentitySet<T>>();
        }
    }
}
