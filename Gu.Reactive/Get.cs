namespace Gu.Reactive
{
    using System;
    using System.Linq.Expressions;

    using Gu.Reactive.PropertyPathStuff;

    public static class Get
    {
        public static TValue ValueOrDefault<TSource, TValue>(
            TSource source,
            Expression<Func<TSource, TValue>> path,
            TValue @default = default (TValue))
        {
            var valuePath = PropertyPath.Create(path);
            var maybe = valuePath.GetValue(source);
            if (maybe.HasValue)
            {
                return maybe.Value;
            }
            return @default;
        }

        public static TValue ValueOrDefault<TValue>(Expression<Func<TValue>> path, TValue @default = default (TValue))
        {
            var valuePath = PropertyPath.Create(path);
            if (valuePath.HasValue)
            {
                return valuePath.Value;
            }
            return @default;
        }

        internal static IValuePath<TSource, TValue> ValuePath<TSource, TValue>(Expression<Func<TSource, TValue>> path)
        {
            var valuePath = PropertyPath.Create(path);
            return valuePath;
        }

        internal static IMaybe<TValue> ValuePath<TValue>(Expression<Func<TValue>> path)
        {
            var valuePath = PropertyPath.Create(path);
            return valuePath;
        }
    }
}
