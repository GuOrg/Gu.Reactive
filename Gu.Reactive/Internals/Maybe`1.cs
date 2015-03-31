namespace Gu.Reactive
{
    internal class Maybe<T> : IMaybe<T>
    {
        public Maybe(bool hasValue, T valueOrDefault)
        {
            HasValue = hasValue;
            ValueOrDefault = valueOrDefault;
        }
        
        public bool HasValue { get; private set; }
        
        public T ValueOrDefault { get; private set; }
    }
}