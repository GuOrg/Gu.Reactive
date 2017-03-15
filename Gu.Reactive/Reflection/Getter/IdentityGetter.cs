namespace Gu.Reactive
{
    internal class IdentityGetter<T> : IGetter<T, T>
    {
        public T GetValue(T source)
        {
            return source;
        }

        public Maybe<T> GetMaybe(T source)
        {
            return Maybe<T>.Some(source);
        }
    }
}