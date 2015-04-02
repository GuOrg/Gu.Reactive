namespace Gu.Reactive.Internals
{
    internal static class MaybeExt
    {
        public static Maybe<T> As<T>(this Maybe<object> maybe)
        {
            var value = maybe.HasValue ? (T)maybe.Value : default(T);
            return new Maybe<T>(maybe.HasValue, value);
        }
    }
}