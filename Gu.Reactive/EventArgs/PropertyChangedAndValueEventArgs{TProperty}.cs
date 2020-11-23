namespace Gu.Reactive
{
    using System.ComponentModel;

    /// <summary>
    /// A <see cref="PropertyChangedEventArgs"/> with the value of the property.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    public class PropertyChangedAndValueEventArgs<TProperty> : PropertyChangedEventArgs, IMaybe<TProperty>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedAndValueEventArgs{TProperty}"/> class.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        public PropertyChangedAndValueEventArgs(string propertyName, TProperty value, bool hasValue)
            : base(propertyName)
        {
            this.Value = value;
            this.HasValue = hasValue;
        }

        internal PropertyChangedAndValueEventArgs(string propertyName, Maybe<TProperty> maybe)
            : this(propertyName, maybe.GetValueOrDefault(), maybe.HasValue)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a value.
        /// Use this to check if the returned value is a default value or read from source.
        /// Example: if subscribing to x => x.Next.Name and Next is null then IsDefaultValue will be true.
        /// If Name is null IsDefaultValue will be false because the value is read from source.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// Gets the value of the property.
        /// This is not guaranteed to be the value when the event was raised in a multi threaded scenario.
        /// </summary>
        public TProperty Value { get; }
    }
}
