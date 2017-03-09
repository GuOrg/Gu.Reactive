namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// The path expression visitor.
    /// </summary>
    internal static class PropertyPathParser
    {
        private static readonly ConcurrentDictionary<LambdaExpression, IReadOnlyList<PropertyInfo>> Cache = new ConcurrentDictionary<LambdaExpression, IReadOnlyList<PropertyInfo>>(PropertyPathComparer.Default);

        internal static IReadOnlyList<PropertyInfo> GetPath<TSource, TResult>(Expression<Func<TSource, TResult>> propertyPath)
        {
            return Cache.GetOrAdd(propertyPath, Create);
        }

        internal static IReadOnlyList<PropertyInfo> GetPath<T>(Expression<Func<T>> propertyPath)
        {
            return Cache.GetOrAdd(propertyPath, Create);
        }

        internal static IReadOnlyList<PropertyInfo> GetPath(LambdaExpression propertyPath)
        {
            return Cache.GetOrAdd(propertyPath, Create);
        }

        private static IReadOnlyList<PropertyInfo> Create(LambdaExpression expression)
        {
            var path = new List<PropertyInfo>();
            var member = expression.GetRootProperty();
            while (member != null)
            {
                path.Add(member.Property());
                member = member.GetPreviousProperty();
            }

            path.Reverse();
            return path;
        }

        private static PropertyInfo Property(this MemberExpression member)
        {
            var property = (PropertyInfo)member.Member;
            var type = member.Expression.Type;
            if (property.ReflectedType == type)
            {
                return property;
            }

            property = type.GetProperty(member.Member.Name);
            if (property != null)
            {
                return property;
            }

            return (PropertyInfo)member.Member;
        }
    }
}
