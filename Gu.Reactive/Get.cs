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
            this TSource source,
            Expression<Func<TSource, TValue>> path,
            TValue @default = default (TValue))
        {
            var valuePath = Internals.ValuePath.Create(path);
            valuePath.Source = source;
            if (valuePath.HasValue)
            {
                return (TValue)valuePath.ValueOrDefault;
            }
            return @default;
        }

        public static TValue ValueOrDefault<TValue>(Expression<Func<TValue>> path, TValue @default = default (TValue))
        {
            var valuePath = Internals.ValuePath.Create(path);
            if (valuePath.HasValue)
            {
                return (TValue)valuePath.ValueOrDefault;
            }
            return @default;
        }

        public static IValuePath<TSource,TValue> ValuePath<TSource, TValue>(Expression<Func<TSource, TValue>> path)
        {
            var valuePath = Internals.ValuePath.Create(path);
            return valuePath.As<TSource, TValue>();
        }

        public static IMaybe<TValue> ValuePath<TValue>(Expression<Func<TValue>> path)
        {
            var valuePath = Internals.ValuePath.Create(path);
            return new Maybe<TValue>(valuePath.HasValue, (TValue)valuePath.ValueOrDefault);
        }
    }
}
