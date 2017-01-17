namespace Gu.Reactive
{
    using System;

    /// <summary>
    /// A factory for mapping values from <typeparamref name="TSource"/> to <typeparamref name="TResult"/>
    /// </summary>
    internal interface IMappingFactory<in TSource, out TResult> : IDisposable
    {
        /// <summary>
        /// True if this factory supports updating index.
        /// </summary>
        bool CanUpdateIndex { get; }

        /// <summary>
        /// Get or create a mappped value for <paramref name="key"/> at position <paramref name="index"/>
        /// </summary>
        TResult GetOrCreateValue(TSource key, int index);

        /// <summary>
        /// Update index for <paramref name="key"/>
        /// </summary>
        TResult UpdateIndex(TSource key, int index);
    }
}