namespace Gu.Reactive.PropertyPathStuff
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using Gu.Reactive.Internals;

    /// <summary>
    /// A comparer for lamda expressions with only properties.
    /// Example x => x.Foo.Bar or () => Foo.Bar
    /// </summary>
    public sealed class PropertyPathComparer : IEqualityComparer<LambdaExpression>
    {
        /// <summary>
        /// The default instance.
        /// </summary>
        public static readonly PropertyPathComparer Default = new PropertyPathComparer();

        private PropertyPathComparer()
        {
        }

        /// <inheritdoc/>
        bool IEqualityComparer<LambdaExpression>.Equals(LambdaExpression x, LambdaExpression y)
        {
            return Equals(x, y);
        }

        /// <inheritdoc/>
        int IEqualityComparer<LambdaExpression>.GetHashCode(LambdaExpression obj)
        {
            return GetHashCode(obj);
        }

        internal static bool Equals(LambdaExpression x, LambdaExpression y)
        {
            var xMember = x.GetRootProperty();
            var yMember = y.GetRootProperty();

            while (xMember != null && yMember != null)
            {
                var xProperty = (PropertyInfo)xMember.Member;
                var yProperty = (PropertyInfo)yMember.Member;
                if (!PropertyInfoComparer.Equals(xProperty, yProperty))
                {
                    return false;
                }

                xMember = xMember.GetPreviousProperty();
                yMember = yMember.GetPreviousProperty();
            }

            return xMember == null && yMember == null && 
                   x.GetSourceType() == y.GetSourceType();
        }

        private static int GetHashCode(LambdaExpression obj)
        {
            var member = obj.GetRootProperty();
            unchecked
            {
                var hash = 19;
                do
                {
                    var property = (PropertyInfo)member.Member;
                    hash = (hash * 397) ^ PropertyInfoComparer.GetHashCode(property);
                    member = member.GetPreviousProperty();
                }
                while (member != null);
                return (hash * 397) ^ obj.GetSourceType().GetHashCode();
            }
        }
    }
}