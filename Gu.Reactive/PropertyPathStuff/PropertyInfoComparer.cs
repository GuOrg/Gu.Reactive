namespace Gu.Reactive.PropertyPathStuff
{
    using System.Collections.Generic;
    using System.Reflection;

    internal class PropertyInfoComparer : IEqualityComparer<PropertyInfo>
    {
        public static readonly PropertyInfoComparer Default = new PropertyInfoComparer();

        private PropertyInfoComparer()
        {
        }

        bool IEqualityComparer<PropertyInfo>.Equals(PropertyInfo x, PropertyInfo y) => Equals(x, y);

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