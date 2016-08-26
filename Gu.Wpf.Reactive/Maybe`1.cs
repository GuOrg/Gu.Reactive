namespace Gu.Wpf.Reactive
{
    using System;

    internal class Maybe<T>
    {
        public static readonly Maybe<T> Empty = new Maybe<T>();
        private readonly T value;

        public Maybe(T value)
        {
            this.value = value;
            this.HasValue = true;
        }

        private Maybe()
        {
            this.HasValue = false;
        }

        public T Value
        {
            get
            {
                if (!this.HasValue)
                {
                    throw new InvalidOperationException("Cannot get Value when HasValue == false. Check before");
                }
                return this.value;
            }
        }

        public bool HasValue { get; }
    }
}