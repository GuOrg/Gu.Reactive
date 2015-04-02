namespace Gu.Reactive.Internals
{
    using System;
    using System.Reflection;

    internal class PathProperty : IPathItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathProperty"/> class.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        public PathProperty(PathProperty previous, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }
            if (!propertyInfo.CanRead)
            {
                throw new ArgumentException("Propert must be readable");
            }
            Previous = previous;
            var pathItem = previous as PathProperty;
            if (pathItem != null)
            {
                if (!previous.PropertyInfo.PropertyType.IsAssignableFrom(propertyInfo.DeclaringType))
                {
                    throw new ArgumentException();
                }
                pathItem.Next = this;
            }
            PropertyInfo = propertyInfo;
        }

        protected PathProperty()
        {
        }

        public PathProperty Next { get; private set; }

        public IPathItem Previous { get; private set; }

        /// <summary>
        /// Gets the property info.
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether is last.
        /// </summary>
        public bool IsLast
        {
            get { return Next == null; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value
        {
            get
            {
                var value = Previous == null
                    ? null
                    : Previous.Value;

                if (value == null)
                {
                    return null;
                }

                return PropertyInfo.GetMethod.Invoke(value, null);
            }
        }

        internal Maybe<object> GetValue(object source)
        {
            if (source == null)
            {
                return new Maybe<object>(false, null);
            }
            if (source.GetType() != PropertyInfo.DeclaringType)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Trying to set source to illegal type. Was: {0} expected {1}",
                        source.GetType()
                              .FullName,
                        PropertyInfo.DeclaringType.FullName));
            }
            var value = PropertyInfo.GetValue(source);
            return new Maybe<object>(false, value);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("PathItem for: {0}.{1}", PropertyInfo.DeclaringType.Name, PropertyInfo.Name);
        }
    }
}