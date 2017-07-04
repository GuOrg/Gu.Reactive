namespace Gu.Reactive.Internals
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal static class SetPool
    {
        public static HashSet<T> Borrow<T>()
            where T : class
        {
            return Pool<T>.Cache.GetOrCreate(() => new HashSet<T>());
        }

        public static void Return<T>(HashSet<T> set)
            where T : class
        {
            set.Clear();
            Pool<T>.Cache.Enqueue(set);
        }

        private static class Pool<T>
            where T : class
        {
            internal static readonly ConcurrentQueue<HashSet<T>> Cache = new ConcurrentQueue<HashSet<T>>();
        }
    }
}