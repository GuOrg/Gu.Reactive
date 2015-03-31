namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ValuePath
    {
        internal static ValuePath<PathItem> CreatePropertyPath<TSource, TValue>(Expression<Func<TSource, TValue>> propertyExpression)
        {
            var path = PropertyPathVisitor.GetPath(propertyExpression);
            var items = CreatePathItems<PathItem>(path.Cast<PropertyInfo>().ToArray(), (pi,info)=> new PathItem(pi,info));
            return new ValuePath<PathItem>(items);
        }

        internal static ValuePath<NotifyingPathItem> CreateNotifyingPropertyPath<TSource, TValue>(Expression<Func<TSource, TValue>> propertyExpression)
        {
            var path = PropertyPathVisitor.GetPath(propertyExpression);
            var items = CreatePathItems<NotifyingPathItem>(path.Cast<PropertyInfo>().ToArray(), (pi,info)=> new NotifyingPathItem(pi,info));
            return new ValuePath<NotifyingPathItem>(items);
        }

        private static IReadOnlyList<T> CreatePathItems<T>(IReadOnlyList<PropertyInfo> properties, Func<T, PropertyInfo, T> creator) where T : PathItem
        {
            var parts = new T[properties.Count];
            T previous = null;
            for (int i = 0; i < properties.Count; i++)
            {
                var propertyInfo = properties[i];
                var item = creator(previous, propertyInfo);
                parts[i] = item;
                previous = item;
            }
            return parts;
        }
    }
}