namespace Gu.Reactive
{
    /// <summary>
    /// A value and the instance having the value.
    /// </summary>
    public struct SourceAndValue<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceAndValue{TValue}"/> struct.
        /// </summary>
        public SourceAndValue(object source, Maybe<TValue> value)
        {
            this.Source = source;
            this.Value = value;
        }

        /// <summary>
        /// The source of the value or the first non-null source in the property path.
        /// </summary>
        public object Source { get; }

        /// <summary>
        /// The value. If the property path is not complete HasValue will be false.
        /// </summary>
        public Maybe<TValue> Value { get; }
    }
}