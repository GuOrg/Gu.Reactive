namespace Gu.Reactive.PropertyPathStuff
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class PropertyPathComparer : IEqualityComparer<LambdaExpression>
    {
        public static readonly PropertyPathComparer Default = new PropertyPathComparer();

        private PropertyPathComparer()
        {
        }

        public bool Equals(LambdaExpression x, LambdaExpression y)
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
                yMember =yMember.GetPreviousProperty();
            }

            return xMember == null && yMember == null;
        }

        public int GetHashCode(LambdaExpression obj)
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
                return hash;
            }
        }
    }
}