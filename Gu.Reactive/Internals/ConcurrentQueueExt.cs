namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Concurrent;

    internal static class ConcurrentQueueExt
    {
        internal static T GetOrCreate<T>(this ConcurrentQueue<T> queue)
            where T : new()
        {
            if (queue.TryDequeue(out var item))
            {
                return item;
            }

            return new T();
        }

        internal static T GetOrCreate<T>(this ConcurrentQueue<T> queue, Func<T> create)
        {
            if (queue.TryDequeue(out var item))
            {
                return item;
            }

            return create();
        }
    }
}
