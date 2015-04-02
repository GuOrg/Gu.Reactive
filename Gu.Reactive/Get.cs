namespace Gu.Reactive
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Internals;

    public static class Get
    {
        public static TValue ValueOrDefault<TSource, TValue>(
            TSource source,
            Expression<Func<TSource, TValue>> path,
            TValue @default = default (TValue))
        {
            var valuePath = Internals.PropertyPath.Create(path);
            var maybe = valuePath.Value(source);
            if (maybe.HasValue)
            {
                return maybe.Value;
            }
            return @default;
        }

        public static TValue ValueOrDefault<TValue>(Expression<Func<TValue>> path, TValue @default = default (TValue))
        {
            var valuePath = Internals.PropertyPath.Create(path);
            if (valuePath.HasValue)
            {
                return valuePath.Value;
            }
            return @default;
        }

        internal static IValuePath<TSource, TValue> ValuePath<TSource, TValue>(Expression<Func<TSource, TValue>> path)
        {
            var valuePath = Internals.PropertyPath.Create(path);
            return valuePath;
        }

        internal static IMaybe<TValue> ValuePath<TValue>(Expression<Func<TValue>> path)
        {
            var valuePath = Internals.PropertyPath.Create(path);
            return valuePath;
        }
    }
}
