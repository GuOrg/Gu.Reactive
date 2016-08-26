namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Reflection;
    using Internals;

    internal sealed class PathProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathProperty"/> class.
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        public PathProperty(PathProperty previous, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (!propertyInfo.CanRead)
            {
                throw new ArgumentException("Propert must be readable");
            }

            this.Previous = previous;
            if (previous != null)
            {
                previous.Next = this;
            }

            this.PropertyInfo = propertyInfo;
        }

        public PathProperty Next { get; private set; }

        public PathProperty Previous { get; }

        /// <summary>
        /// Gets the property info.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets or sets a value indicating whether is last.
        /// </summary>
        public bool IsLast => this.Next == null;

        /// <summary>
        /// Gets value all the way from the root recursively.
        /// Checks for null along the way.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal Maybe<T> GetValue<T>(object source)
        {
            if (source == null)
            {
                return new Maybe<T>(false, default(T));
            }

            if (this.Previous == null)
            {
                var o = this.PropertyInfo.GetValue(source);
                return new Maybe<T>(true, (T)o);
            }

            var maybe = this.Previous.GetValue<object>(source);
            if (!maybe.HasValue)
            {
                return maybe.As<T>();
            }

            if (maybe.Value == null)
            {
                return new Maybe<T>(false, default(T));
            }

            var value = (T)this.PropertyInfo.GetValue(maybe.Value);
            return new Maybe<T>(true, value);
        }

        public override string ToString()
        {
            return $"PathItem for: {this.PropertyInfo.DeclaringType.PrettyName()}.{this.PropertyInfo.Name}";
        }
    }
}