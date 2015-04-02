namespace Gu.Reactive.Internals
{
    internal static class MaybeExt
    {
        public static Maybe<T> As<T>(this Maybe<object> maybe)
        {
            return new Maybe<T>(maybe.HasValue, (T)maybe.Value);
        }
    }
}