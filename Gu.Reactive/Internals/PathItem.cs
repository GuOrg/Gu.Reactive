namespace Gu.Reactive.Internals
{
    using System;
    using System.Reflection;

    internal class PathItem : IPathItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathItem"/> class.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        public PathItem(IPathItem previous, PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead)
            {
                throw new ArgumentException("Propert must be readable");
            }
            Previous = previous;
            var pathItem = previous as PathItem;
            if (pathItem != null)
            {
                pathItem.Next = this;
            }
            PropertyInfo = propertyInfo;
        }

        public PathItem Next { get; private set; }

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

        internal object GetValue(object source)
        {
            if (source == null)
            {
                return null;
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
            return PropertyInfo.GetValue(source);
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