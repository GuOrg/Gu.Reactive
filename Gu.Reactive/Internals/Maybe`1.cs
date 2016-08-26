namespace Gu.Reactive.Internals
{
    using System;

    internal class Maybe<T> : IMaybe<T>
    {
        private readonly T _value;

        public Maybe(bool hasValue, T value)
        {
            HasValue = hasValue;
            _value = value;
        }

        public bool HasValue { get; }

        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException("Check HasValue before calling. This instance has no value.");
                }

                return _value;
            }
        }
    }
}