namespace Gu.Reactive
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    /// <summary>
    /// Helper methods for <see cref="Chunk{T}"/>
    /// </summary>
    public static class Chunk
    {
        /// <summary>
        /// Create a <see cref="Chunk{T}"/> with one item
        /// </summary>
        public static Chunk<T> One<T>(T item)
        {
            return new Chunk<T>(TimeSpan.Zero, ImmediateScheduler.Instance) { item };
        }

        /// <summary>
        /// Add the notifications in source to <paramref name="chunk"/>
        /// Then throttle using <see cref="Chunk{T}.BufferTime"/> and <see cref="Chunk{T}.Scheduler"/>
        /// Note that this mutates <paramref name="chunk"/> so it must be cleared somewhere.
        /// </summary>
        public static IObservable<Chunk<T>> AsSlidingChunk<T>(this IObservable<T> source, Chunk<T> chunk)
        {
            return source.Scan(chunk, (c, item) => c.Add(item))
                         .Publish(
                             shared => chunk.ObserveValue(x => x.BufferTime, true)
                                            .Select(
                                                bt => bt.Value == TimeSpan.Zero
                                                          ? shared
                                                          : shared.Throttle(chunk.BufferTime, chunk.Scheduler))
                                            .Switch());
        }
    }
}