namespace Gu.Reactive
{
    /// <summary>
    /// Extension methods for maybe types.
    /// </summary>
    public static class MaybeExt
    {
        /// <summary>
        /// Cast to Maybe{T}
        /// </summary>
        public static Maybe<T> Cast<T>(this Maybe<object> maybe)
        {
            return maybe.HasValue
                       ? Maybe<T>.Some((T)maybe.Value)
                       : Maybe<T>.None;
        }

        /// <summary>
        /// Cast to Maybe{T}
        /// </summary>
        public static Maybe<T> Cast<T>(this IMaybe<object> maybe)
        {
            return maybe.HasValue
                       ? Maybe<T>.Some((T)maybe.Value)
                       : Maybe<T>.None;
        }
    }
}