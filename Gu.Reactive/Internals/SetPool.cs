namespace Gu.Reactive.Internals
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    internal static class SetPool
    {
        public static IdentitySet<T> Borrow<T>()
            where T : class
        {
            return Pool<T>.Cache.GetOrCreate(() => new IdentitySet<T>());
        }

        public static void Return<T>(IdentitySet<T> set)
            where T : class
        {
            set.Clear();
            Pool<T>.Cache.Enqueue(set);
        }

        internal class IdentitySet<T> : HashSet<T>
        {
            public IdentitySet()
                : base(ObjectIdentityComparer<T>.Default)
            {
            }

            public T AddAndReturn(T item)
            {
                if (item != null)
                {
                    this.Add(item);
                }

                return item;
            }
        }

        private static class Pool<T>
            where T : class
        {
            internal static readonly ConcurrentQueue<IdentitySet<T>> Cache = new ConcurrentQueue<IdentitySet<T>>();
        }
    }
}