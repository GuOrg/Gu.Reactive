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
            return lambda.Body switch
            {
                UnaryExpression { Operand: MemberExpression { Member: PropertyInfo _ } memberExpression } unary
                when IsConversion(unary)
                => memberExpression,
                MemberExpression { Member: PropertyInfo _ } memberExpression => memberExpression,
                MemberExpression member => throw new ArgumentException($"Expected path to be properties only. Was {lambda}. Unexpected item: {member.Member}"),
                _ => throw new ArgumentException($"Expected path to be properties only. Was {lambda}"),
            };

            bool IsConversion(UnaryExpression unary)
            {
                return unary switch
                {
                    { NodeType: ExpressionType.Convert } => true,
                    { NodeType: ExpressionType.ConvertChecked } => true,
                    _ => false,
                };
            }
        }

        internal static MemberExpression? GetPreviousProperty(this MemberExpression parent)
        {
            if (parent.Expression is null ||
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
            while (property is { Expression: MemberExpression expression, Member: PropertyInfo _ })
            {
                property = expression;
            }

            return property switch
            {
                { Expression: ConstantExpression constant }
                => IsCompilerGenerated(property.Member)
                    ? property.Type
                    : constant.Type,
                { Expression: ParameterExpression parameter }
                => parameter.Type,
                _ => throw new ArgumentException("Could not determine source type."),
            };
        }

        private static bool IsCompilerGenerated(MemberInfo member)
        {
            return member?.ReflectedType?.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false) == true;
        }
    }
}
