namespace Gu.Reactive.Internals
{
    using System;
    using System.Reflection;

    internal class PathProperty
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
            if (previous != null)
            {
                if (!previous.PropertyInfo.PropertyType.IsAssignableFrom(propertyInfo.DeclaringType))
                {
                    throw new ArgumentException();
                }
                previous.Next = this;
            }
            PropertyInfo = propertyInfo;
        }

        protected PathProperty()
        {
        }

        public PathProperty Next { get; private set; }

        public PathProperty Previous { get; private set; }

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
            if (Previous == null)
            {
                if (source.GetType() != PropertyInfo.DeclaringType)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Trying to set source to illegal type. Was: {0} expected {1}",
                            source.GetType()
                                  .FullName,
                            PropertyInfo.DeclaringType.FullName));
                }
                var o = PropertyInfo.GetValue(source);
                return new Maybe<T>(true,(T)o);
            }
            var maybe = Previous.GetValue<object>(source);
            if (!maybe.HasValue)
            {
                return maybe.As<T>();
            }
            if (maybe.Value == null)
            {
                return new Maybe<T>(false, default(T));
            }
            var value = (T)PropertyInfo.GetValue(maybe.Value);
            return new Maybe<T>(true, value);
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