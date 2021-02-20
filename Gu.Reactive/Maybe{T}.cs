namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Similar to Nullable{T} but for any type.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public readonly struct Maybe<T> : IMaybe<T>, IEquatable<Maybe<T>>
    {
        private readonly T? value;

        private Maybe(bool hasValue, T? value)
        {
            this.HasValue = hasValue;
            this.value = value;
        }

#pragma warning disable CA1000 // Do not declare static members on generic types
        /// <summary>
        /// Gets the default instance when value is missing.
        /// </summary>
        public static Maybe<T> None { get; } = new Maybe<T>(hasValue: false, value: default!);
#pragma warning restore CA1000 // Do not declare static members on generic types

        /// <inheritdoc />
        public bool HasValue { get; }

        /// <inheritdoc />
        public T? Value
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

        /// <summary>Check if <paramref name="left"/> is equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="Maybe{T}"/>.</param>
        /// <param name="right">The right <see cref="Maybe{T}"/>.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>Check if <paramref name="left"/> is not equal to <paramref name="right"/>.</summary>
        /// <param name="left">The left <see cref="Maybe{T}"/>.</param>
        /// <param name="right">The right <see cref="Maybe{T}"/>.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !left.Equals(right);
        }

#pragma warning disable CA1000 // Do not declare static members on generic types
        /// <summary>
        /// Create an instance with a value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="Maybe{T}"/>.</returns>
        public static Maybe<T> Some(T? value) => new Maybe<T>(hasValue: true, value: value);
#pragma warning restore CA1000 // Do not declare static members on generic types

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
        public override bool Equals(object? obj)
        {
            return obj is Maybe<T> maybe &&
                   this.Equals(maybe);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.HasValue
                ? this.value is { } x ? EqualityComparer<T>.Default.GetHashCode(x) : -1
                : 0;
        }

        /// <summary>
        /// Get the value if HasValue is true and default(T) if not.
        /// </summary>
        /// <returns><see cref="Value"/> or default.</returns>
        public T? GetValueOrDefault() => this.HasValue ? this.value : default;

        /// <summary>
        /// Get the value if HasValue is true and <paramref name="defaultValue"/> if not.
        /// </summary>
        /// <param name="defaultValue">The value if HasValue == false.</param>
        /// <returns><see cref="Value"/> or <paramref name="defaultValue"/>.</returns>
        public T? GetValueOrDefault(T? defaultValue) => this.HasValue ? this.value : defaultValue;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this switch
            {
                { HasValue: false } => "None",
                { value: null } => "Some null",
                _ => $"Some {this.value}",
            };
        }
    }
}
