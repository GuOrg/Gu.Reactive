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
        /// Add the notifications in source to <paramref name="chunk"/>
        /// Then throttle using <see cref="Chunk{T}.BufferTime"/> and <see cref="Chunk{T}.Scheduler"/>
        /// Note that this mutates <paramref name="chunk"/> so it must be cleared somewhere.
        /// </summary>
        public static IObservable<Chunk<T>> Slide<T>(this IObservable<T> source, Chunk<T> chunk)
        {
            return source.Scan(chunk, (c, item) => c.Add(item))
                         .Publish(
                             shared => chunk.ObserveValue(x => x.BufferTime)
                                            .Select(
                                                bt => bt.Value == TimeSpan.Zero
                                                    ? shared
                                                    : shared.Throttle(chunk.BufferTime, chunk.Scheduler))
                                            .StartWith(Observable.Return(chunk)))
                         .Switch()
                         .Where(c => c.Count > 0);
        }
    }
}