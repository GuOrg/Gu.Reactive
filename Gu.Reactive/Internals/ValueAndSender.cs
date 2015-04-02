namespace Gu.Reactive.Internals
{
    internal class ValueAndSender<TValue>
    {
        public ValueAndSender(object source, Maybe<TValue> value)
        {
            Source = source;
            Value = value;
        }
        
        public object Source { get; private set; }
        
        public Maybe<TValue> Value { get; private set; }
    }
}