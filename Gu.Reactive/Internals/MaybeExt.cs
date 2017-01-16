namespace Gu.Reactive.Internals
{
    internal static class MaybeExt
    {
        public static Maybe<T> As<T>(this Maybe<object> maybe)
        {
            return maybe.HasValue
                       ? Maybe<T>.Some((T)maybe.Value)
                       : Maybe<T>.None;
        }
    }
}