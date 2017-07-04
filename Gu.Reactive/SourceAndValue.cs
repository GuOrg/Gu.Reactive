namespace Gu.Reactive
{
    /// <summary>
    /// Factory methods for <see cref="SourceAndValue{TSource,TValue}"/>
    /// </summary>
    public static class SourceAndValue
    {
        /// <summary>
        /// Create a new instance of the <see cref="SourceAndValue{TSource,TValue}"/> struct.
        /// </summary>
        public static SourceAndValue<TSource, TValue> Create<TSource, TValue>(TSource source, Maybe<TValue> value) => new SourceAndValue<TSource, TValue>(source, value);
    }
}