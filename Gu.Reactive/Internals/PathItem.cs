namespace Gu.Reactive.Internals
{
    using System;
    using System.Reflection;

    internal class PathItem
    {
        protected readonly WeakReference _sourceRef = new WeakReference(null);
        
        /// <summary>
        /// Initializes a new instance of the <see cref="PathItem"/> class.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        public PathItem(object source, PropertyInfo propertyInfo)
        {
            _sourceRef.Target = source;
            PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathItem"/> class.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        public PathItem(PathItem previous, PropertyInfo propertyInfo)
        {
            Previous = previous;
            previous.Next = this;
            PropertyInfo = propertyInfo;
        }

        public PathItem Next { get; private set; }

        public PathItem Previous { get; private set; }

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
                    ? _sourceRef.Target 
                    : this.Previous.Value;

                if (value == null)
                {
                    return null;
                }

                return PropertyInfo.GetMethod.Invoke(value, null);
            }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}.{1}", PropertyInfo.DeclaringType.Name, PropertyInfo.Name);
        }
    }
}