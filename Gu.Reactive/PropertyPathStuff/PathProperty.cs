namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Reflection;

    using Internals;

    internal sealed class PathProperty
    {
        private readonly IGetter getter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathProperty"/> class.
        /// </summary>
        /// <param name="previous">The preivous property in the <see cref="PropertyPath"/></param>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        public PathProperty(PathProperty previous, PropertyInfo propertyInfo)
        {
            Ensure.NotNull(propertyInfo, nameof(propertyInfo));

            if (!propertyInfo.CanRead)
            {
                var message = string.Format(
                    "Propert cannot be write only." + Environment.NewLine +
                    "The property {0}.{1}.{2} does not have a getter.",
                    propertyInfo.ReflectedType?.Namespace,
                    propertyInfo.ReflectedType?.PrettyName(),
                    propertyInfo.Name);
                throw new ArgumentException(message, nameof(propertyInfo));
            }

            this.Previous = previous;
            this.PropertyInfo = propertyInfo;
            this.getter = Getter.GetOrCreate(propertyInfo);
        }

        public PathProperty Previous { get; }

        /// <summary>
        /// Gets the property info.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        public override string ToString() => $"PathItem for: {this.PropertyInfo.DeclaringType.PrettyName()}.{this.PropertyInfo.Name}";

        internal Maybe<object> GetPropertyValue(object source) => source == null
                                                                      ? Maybe<object>.None
                                                                      : Maybe<object>.Some(this.getter.GetValue(source));

        /// <summary>
        /// Gets value all the way from the root recursively.
        /// Checks for null along the way.
        /// </summary>
        /// <param name="rootSource">The source object</param>
        /// <returns>The value of the property.</returns>
        internal Maybe<T> GetValueFromRoot<T>(object rootSource)
        {
            if (rootSource == null)
            {
                return Maybe<T>.None;
            }

            if (this.Previous == null)
            {
                var o = this.getter.GetValue(rootSource);
                return Maybe<T>.Some((T)o);
            }

            var maybe = this.Previous.GetValueFromRoot<object>(rootSource);
            if (maybe.ValueOrDefault() == null)
            {
                return Maybe<T>.None;
            }

            var value = (T)this.getter.GetValue(maybe.Value);
            return Maybe<T>.Some(value);
        }
    }
}