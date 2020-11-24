namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// A factory for mapping values from <typeparamref name="TSource"/> to <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TResult">The mapped type.</typeparam>
    internal interface IMapper<in TSource, TResult> : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this factory supports updating index.
        /// </summary>
        bool CanUpdateIndex { get; }

        /// <summary>
        /// Get or create a mapped value for <paramref name="key"/> at position <paramref name="index"/>.
        /// </summary>
        /// <param name="key">The source value.</param>
        /// <param name="index">The index.</param>
        /// <returns>The mapped value.</returns>
        TResult GetOrCreate(TSource key, int index);

        /// <summary>
        /// Update index for <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The source value.</param>
        /// <param name="oldResult">The old mapped value.</param>
        /// <param name="index">The index.</param>
        /// <returns>The mapped value.</returns>
        TResult Update(TSource key, TResult oldResult, int index);

        /// <summary>
        /// For removing items from cache.
        /// </summary>
        /// <param name="source">The source value.</param>
        /// <param name="mapped">The mapped value.</param>
        void Remove(TSource source, TResult mapped);

        /// <summary>
        /// Start a refresh transaction. This happens when the wrapped collection signals Reset.
        /// </summary>
        /// <returns>A transaction that locks the instance until disposed.</returns>
        IDisposable RefreshTransaction();
    }
}
