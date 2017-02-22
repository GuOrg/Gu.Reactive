namespace Gu.Reactive.Internals
{
    internal struct SenderAndValue<TValue>
    {
        public SenderAndValue(object source, Maybe<TValue> value)
        {
            this.Source = source;
            this.Value = value;
        }

        public object Source { get; }

        public Maybe<TValue> Value { get; }
    }
}