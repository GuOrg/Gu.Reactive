namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using Gu.Reactive.Internals;

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
                path.Add((PropertyInfo)member.Member);
                member = member.GetPreviousProperty();
            }

            path.Reverse();
            return path;
        }
    }
}
