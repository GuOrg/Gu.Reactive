namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Similar to Nullable{T} but for any type.
    /// </summary>
    public struct Maybe<T> : IMaybe<T>, IEquatable<Maybe<T>>
    {
        private readonly T value;

        private Maybe(bool hasValue, T value)
        {
            this.HasValue = hasValue;
            this.value = value;
        }

        /// <summary>
        /// The default instance when value is missing.
        /// </summary>
        public static Maybe<T> None => new Maybe<T>(hasValue: false, value: default(T));

        /// <inheritdoc />
        public bool HasValue { get; }

        /// <inheritdoc />
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

        /// <summary>
        /// Compare <paramref name="left"/> and <paramref name="right"/> for equality.
        /// </summary>
        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compare <paramref name="left"/> and <paramref name="right"/> for inequality.
        /// </summary>
        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Create an instance with a value.
        /// </summary>
        public static Maybe<T> Some(T value) => new Maybe<T>(hasValue: true, value: value);

        /// <inheritdoc />
        public bool Equals(Maybe<T> other)
        {
            if (this.HasValue != other.HasValue)
            {
                return false;
            }

            return !this.HasValue || EqualityComparer<T>.Default.Equals(this.value, other.value);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Maybe<T> maybe &&
                   this.Equals(maybe);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.HasValue
                ? EqualityComparer<T>.Default.GetHashCode(this.value)
                : 0;
        }

        /// <summary>
        /// Get the value if HasValue is true and default(T) if not.
        /// </summary>
        public T GetValueOrDefault()
        {
            return this.value;
        }

        /// <summary>
        /// Get the value if HasValue is true and <paramref name="defaultValue"/> if not.
        /// </summary>
        public T GetValueOrDefault(T defaultValue)
        {
            return this.HasValue ? this.value : defaultValue;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.HasValue
                       ? $"Some {this.Value?.ToString() ?? "null"}"
                       : "None";
        }
    }
}
