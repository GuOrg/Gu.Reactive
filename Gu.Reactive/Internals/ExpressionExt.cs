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

            var member = lambda.Body as MemberExpression;
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

            var me = parent.Expression as MemberExpression;
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

        internal static Type GetSourceType(this LambdaExpression lamda)
        {
            var property = lamda.GetRootProperty();
            while (property.Expression is MemberExpression && property.Member is PropertyInfo)
            {
                property = (MemberExpression)property.Expression;
            }

            var constant = property.Expression as ConstantExpression;
            if (constant != null)
            {
                if (IsCompilerGenerated(property.Member))
                {
                    return property.Type;
                }

                return constant.Type;
            }

            var parameter = property.Expression as ParameterExpression;
            if (parameter != null)
            {
                return parameter.Type;
            }

            throw new ArgumentException("Could not determine source type.");
        }

        internal static object GetSourceValue<TValue>(this Expression<Func<TValue>> lamda)
        {
            var property = lamda.GetRootProperty();
            while (property.Expression is MemberExpression && property.Member is PropertyInfo)
            {
                property = (MemberExpression)property.Expression;
            }

            var constant = property.Expression as ConstantExpression;
            if (constant != null)
            {
                if (IsCompilerGenerated(property.Member))
                {
                    return ((PropertyInfo)property.Member).GetValueViaDelegate(constant.Value);
                }

                return constant.Value;
            }

            throw new ArgumentException("Expected path to have a constant.");
        }

        private static bool IsCompilerGenerated(MemberInfo member)
        {
            return member?.ReflectedType?.IsDefined(typeof(CompilerGeneratedAttribute), false) == true;
        }
    }
}