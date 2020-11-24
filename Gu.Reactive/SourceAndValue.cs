namespace Gu.Reactive
{
    /// <summary>
    /// Factory methods for <see cref="SourceAndValue{TSource,TValue}"/>.
    /// </summary>
    public static class SourceAndValue
    {
        /// <summary>
        /// Create a new instance of the <see cref="SourceAndValue{TSource,TValue}"/> struct.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="value">The <see cref="Maybe{TValue}"/>.</param>
        /// <returns>A <see cref="SourceAndValue{TSource,TValue}"/>.</returns>
        public static SourceAndValue<TSource, TValue> Create<TSource, TValue>(TSource source, Maybe<TValue> value) => new SourceAndValue<TSource, TValue>(source, value);
    }
}
