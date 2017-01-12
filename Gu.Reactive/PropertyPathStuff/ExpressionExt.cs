namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class ExpressionExt
    {
        internal static MemberExpression GetRootProperty(this LambdaExpression lambda)
        {
            switch (lambda.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    {
                        var convert = (UnaryExpression)lambda.Body;
                        var memberExpression = convert.Operand as MemberExpression;
                        if (memberExpression == null)
                        {
                            throw new ArgumentException($"Expected path to be properties only. Was {lambda}");
                        }

                        if (memberExpression.Member is PropertyInfo)
                        {
                            return memberExpression;
                        }

                        throw new ArgumentException($"Expected path to be properties only. Was {lambda}. Unexpected item: {memberExpression.Member}");
                    }
            }

            var member = lambda.Body as System.Linq.Expressions.MemberExpression;
            if (member == null)
            {
                throw new ArgumentException($"Expected path to be properties only. Was {lambda}");
            }

            if (member.Member is PropertyInfo)
            {
                return member;
            }

            throw new ArgumentException($"Expected path to be properties only. Was {lambda}. Unexpected item: {member.Member}");
        }

        internal static MemberExpression GetPreviousProperty(this MemberExpression parent)
        {
            if (parent.Expression == null ||
                parent.Expression.NodeType == ExpressionType.Parameter ||
                parent.Expression.NodeType == ExpressionType.Constant)
            {
                return null;
            }

            var me = parent.Expression as System.Linq.Expressions.MemberExpression;
            if (me == null)
            {
                throw new ArgumentException($"Expected path to be properties only. Was {parent}");
            }

            if (me.Member is PropertyInfo)
            {
                return me;
            }

            if (IsCompilerGenerated(me.Member))
            {
                return null;
            }

            throw new ArgumentException($"Expected path to be properties only. Was: {parent}. Unexpected item: {me.Member}");
        }

        private static bool IsCompilerGenerated(MemberInfo member)
        {
            return member?.ReflectedType?.IsDefined(typeof(CompilerGeneratedAttribute), false) == true;
        }
    }
}