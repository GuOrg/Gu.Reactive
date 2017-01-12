namespace Gu.Reactive.Internals
{
    using System;

    internal struct Maybe<T> : IMaybe<T>
    {
        private readonly T value;

        public Maybe(bool hasValue, T value)
        {
            this.HasValue = hasValue;
            this.value = value;
        }

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
    }
}