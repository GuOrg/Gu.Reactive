namespace Gu.Reactive.PropertyPathStuff
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// A comparer for using <see cref="PropertyInfo"/> as key.
    /// </summary>
    public sealed class PropertyInfoComparer : IEqualityComparer<PropertyInfo>
    {
        /// <summary>
        /// The default instance.
        /// </summary>
        public static readonly PropertyInfoComparer Default = new PropertyInfoComparer();

        private PropertyInfoComparer()
        {
        }

        /// <inheritdoc/>
        bool IEqualityComparer<PropertyInfo>.Equals(PropertyInfo x, PropertyInfo y) => Equals(x, y);

        /// <inheritdoc/>
        int IEqualityComparer<PropertyInfo>.GetHashCode(PropertyInfo obj) => GetHashCode(obj);

        internal static bool Equals(PropertyInfo x, PropertyInfo y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.MetadataToken == y.MetadataToken &&
                   x.Module.MetadataToken == y.Module.MetadataToken;
        }

        internal static int GetHashCode(PropertyInfo obj)
        {
            unchecked
            {
                return (obj.MetadataToken * 397) ^ obj.Module.MetadataToken;
            }
        }
    }
}