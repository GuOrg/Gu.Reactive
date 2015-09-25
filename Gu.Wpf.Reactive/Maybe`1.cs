namespace Gu.Wpf.Reactive
{
    using System;

    internal class Maybe<T>
    {
        public static readonly Maybe<T> Empty = new Maybe<T>();
        private T _value;

        public Maybe(T value)
        {
            _value = value;
            HasValue = true;
        }

        private Maybe()
        {
            HasValue = false;
        }

        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException("Cannot get Value when HasValue == false. Check before");
                }
                return _value;
            }
        }

        public bool HasValue { get; }
    }
}