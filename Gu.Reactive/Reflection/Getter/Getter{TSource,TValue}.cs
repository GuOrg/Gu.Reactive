namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Reflection;

    using Gu.Reactive.Internals;

    /// <summary>
    /// A wrapper around a delegate created from a <see cref="PropertyInfo.GetMethod"/>
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TValue">The property type.</typeparam>
    public abstract class Getter<TSource, TValue> : IGetter<TSource, TValue>, IGetter<TValue>, IGetter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Getter{TSource, TValue}"/> class.
        /// </summary>
        protected Getter(PropertyInfo property)
        {
            Ensure.NotNull(property, nameof(property));
            Ensure.Equal(typeof(TSource), property.ReflectedType, nameof(property));
            Ensure.Equal(typeof(TValue), property.PropertyType, nameof(property));
            this.Property = property;
        }

        /// <summary>
        /// The <see cref="PropertyInfo"/> that this instance is for.
        /// </summary>
        public PropertyInfo Property { get; }

        // ReSharper disable once UnusedMember.Local for inspection  of the cache in the debugger.
        private IReadOnlyList<Getter.CacheItem> CacheDebugView => Getter.CacheDebugView;

        /// <inheritdoc/>
        object IGetter.GetValue(object source) => this.GetValue((TSource)source);

        /// <inheritdoc />
        public abstract TValue GetValue(TSource source);

        /// <inheritdoc />
        public Maybe<TValue> GetMaybe(TSource source) => source == null
                                                             ? Maybe<TValue>.None
                                                             : Maybe<TValue>.Some(this.GetValue(source));

        /// <inheritdoc/>
        Maybe<object> IGetter.GetMaybe(object source) => source == null
                                                             ? Maybe<object>.None
                                                             : Maybe<object>.Some(this.GetValue((TSource)source));

        /// <inheritdoc/>
        Maybe<TValue> IGetter<TValue>.GetMaybe(object source) => this.GetMaybe((TSource)source);

        /// <inheritdoc/>
        public override string ToString() => $"{this.Property.ReflectedType?.Name}.{this.Property.Name}";
    }
}