namespace Gu.Reactive.Internals
{
    using System;

    internal struct Maybe<T> : IMaybe<T>
    {
        private readonly T value;

        private Maybe(bool hasValue, T value)
        {
            this.HasValue = hasValue;
            this.value = value;
        }

        public static Maybe<T> None => new Maybe<T>(false, default(T));

        /// <inheritdoc/>
        public bool HasValue { get; }

        /// <inheritdoc/>
        public T Value
        {
            get
            {
                if (!this.HasValue)
                {
                    throw new InvalidOperationException("Check HasValue before calling. This instance has no value.");
                }

                return this.value;
            }
        }

        public static Maybe<T> Some(T value) => new Maybe<T>(true, value);

        public T ValueOrDefault()
        {
            return this.HasValue
                       ? this.value
                       : default(T);
        }
    }
}