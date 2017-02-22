#pragma warning disable 1591
#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Reactive
{
    using System;
    using System.Linq.Expressions;

    using Gu.Reactive.Internals;

    [Obsolete("Use C#6 ?. operator")]
    public static class Get
    {
        [Obsolete("Use C#6 ?. operator")]
        public static TValue ValueOrDefault<TSource, TValue>(
            TSource source,
            Expression<Func<TSource, TValue>> path,
            TValue @default = default(TValue))
        {
            var valuePath = (IValuePath<TSource, TValue>)PropertyPath.GetOrCreate(path);
            var maybe = valuePath.GetValue(source);
            if (maybe.HasValue)
            {
                return maybe.Value;
            }

            return @default;
        }

        [Obsolete("Use C#6 ?. operator")]
        public static TValue ValueOrDefault<TValue>(Expression<Func<TValue>> path, TValue @default = default(TValue))
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
            return PropertyPath.GetOrCreate(path);
        }
    }
}
