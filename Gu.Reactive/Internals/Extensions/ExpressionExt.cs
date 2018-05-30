namespace Gu.Reactive.Internals
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
                        if (convert.Operand is MemberExpression memberExpression)
                        {
                            if (memberExpression.Member is PropertyInfo)
                            {
                                return memberExpression;
                            }

                            throw new ArgumentException(
                                $"Expected path to be properties only. Was {lambda}. Unexpected item: {memberExpression.Member}");
                        }

                        throw new ArgumentException($"Expected path to be properties only. Was {lambda}");
                    }
            }

            if (lambda.Body is MemberExpression member)
            {
                if (member.Member is PropertyInfo)
                {
                    return member;
                }

                throw new ArgumentException(
                    $"Expected path to be properties only. Was {lambda}. Unexpected item: {member.Member}");
            }

            throw new ArgumentException($"Expected path to be properties only. Was {lambda}");
        }

        internal static MemberExpression GetPreviousProperty(this MemberExpression parent)
        {
            if (parent.Expression == null ||
                parent.Expression.NodeType == ExpressionType.Parameter ||
                parent.Expression.NodeType == ExpressionType.Constant)
            {
                return null;
            }

            if (parent.Expression is MemberExpression me)
            {
                if (me.Member is PropertyInfo)
                {
                    return me;
                }

                if (IsCompilerGenerated(me.Member))
                {
                    return null;
                }

                throw new ArgumentException(
                    $"Expected path to be properties only. Was: {parent}. Unexpected item: {me.Member}");
            }

            throw new ArgumentException($"Expected path to be properties only. Was {parent}");
        }

        internal static Type GetSourceType(this LambdaExpression lambda)
        {
            var property = lambda.GetRootProperty();
            while (property.Expression is MemberExpression && property.Member is PropertyInfo)
            {
                property = (MemberExpression)property.Expression;
            }

            if (property.Expression is ConstantExpression constant)
            {
                if (IsCompilerGenerated(property.Member))
                {
                    return property.Type;
                }

                return constant.Type;
            }

            if (property.Expression is ParameterExpression parameter)
            {
                return parameter.Type;
            }

            throw new ArgumentException("Could not determine source type.");
        }

        private static bool IsCompilerGenerated(MemberInfo member)
        {
            return member?.ReflectedType?.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false) == true;
        }
    }
}
