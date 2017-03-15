namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// A factory for mapping values from <typeparamref name="TSource"/> to <typeparamref name="TResult"/>
    /// </summary>
    internal interface IMapper<in TSource, TResult> : IDisposable
    {
        /// <summary>
        /// True if this factory supports updating index.
        /// </summary>
        bool CanUpdateIndex { get; }

        /// <summary>
        /// Get or create a mapped value for <paramref name="key"/> at position <paramref name="index"/>
        /// </summary>
        TResult GetOrCreate(TSource key, int index);

        /// <summary>
        /// Update index for <paramref name="key"/>
        /// </summary>
        TResult Update(TSource key, TResult oldResult, int index);

        /// <summary>
        /// For removing items from cache.
        /// </summary>
        void Remove(TSource source, TResult mapped);

        /// <summary>
        /// Start a refresh transcation. This happens when the wrapped collection signals Reset
        /// </summary>
        IDisposable RefreshTransaction();
    }
}