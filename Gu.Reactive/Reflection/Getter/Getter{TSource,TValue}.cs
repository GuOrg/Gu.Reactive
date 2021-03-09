namespace Gu.Reactive
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    /// <summary>
    /// A wrapper around a delegate created from a <see cref="PropertyInfo.GetMethod"/>.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TValue">The property type.</typeparam>
    public abstract class Getter<TSource, TValue> : IGetter<TSource, TValue>, IGetter<TValue>, IGetter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Getter{TSource, TValue}"/> class.
        /// </summary>
        /// <param name="property">The <see cref="PropertyInfo"/>.</param>
        protected Getter(PropertyInfo property)
        {
            if (property is null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            if (property.ReflectedType != typeof(TSource))
            {
                throw new ArgumentException("ReflectedType != typeof(TSource)", nameof(property));
            }

            if (property.PropertyType != typeof(TValue))
            {
                throw new ArgumentException("PropertyType != typeof(TValue)", nameof(property));
            }

            this.Property = property;
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> that this instance is for.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <inheritdoc/>
        object? IGetter.GetValue(object source) => this.GetValue((TSource)source);

        /// <inheritdoc />
        public abstract TValue GetValue(TSource source);

        /// <inheritdoc />
        public Maybe<TValue> GetMaybe([AllowNull]TSource source) => source is null
            ? Maybe<TValue>.None
            : Maybe<TValue>.Some(this.GetValue(source));

        /// <inheritdoc/>
        Maybe<object?> IGetter.GetMaybe(object? source) => source is null
            ? Maybe<object?>.None
            : Maybe<object?>.Some(this.GetValue((TSource)source));

        /// <inheritdoc/>
        Maybe<TValue> IGetter<TValue>.GetMaybe(object? source) => this.GetMaybe((TSource?)source);

        /// <inheritdoc/>
        public override string ToString() => $"{this.Property.ReflectedType?.Name}.{this.Property.Name}";
    }
}
