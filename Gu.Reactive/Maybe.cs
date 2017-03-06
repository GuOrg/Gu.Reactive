namespace Gu.Reactive
{
    /// <summary>
    /// Extension methods for maybe types.
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        /// Create an instance with a value.
        /// </summary>
        public static Maybe<T> Some<T>(T value) => Maybe<T>.Some(value);

        /// <summary>
        /// The default instance when value is missing.
        /// </summary>
        public static Maybe<T> None<T>() => Maybe<T>.None;

        /// <summary>
        /// Cast to Maybe{T}
        /// </summary>
        public static Maybe<T> Cast<T>(this Maybe<object> maybe)
        {
            return maybe.HasValue
                       ? Some((T)maybe.Value)
                       : Maybe<T>.None;
        }

        /// <summary>
        /// Cast to Maybe{T}
        /// </summary>
        public static Maybe<T> Cast<T>(this IMaybe<object> maybe)
        {
            return maybe.HasValue
                       ? Some((T)maybe.Value)
                       : Maybe<T>.None;
        }
    }
}