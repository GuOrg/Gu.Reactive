namespace Gu.Reactive.Internals
{
    public interface IValuePath<TValue>
    {
        Maybe<TValue> Value { get; }
    }

    public interface IValuePath<in TSource, out TValue>
    {
        bool HasValue(TSource source);
        TValue Value(TSource source);
    }

    public struct Maybe<T>
    {
        public readonly bool HasValue;
        
        public readonly T ValueOrDefault;
        
        public Maybe(bool hasValue, T valueOrDefault)
        {
            HasValue = hasValue;
            ValueOrDefault = valueOrDefault;
        }
    }
}