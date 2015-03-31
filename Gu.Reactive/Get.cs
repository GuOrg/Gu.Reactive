namespace Gu.Reactive
{
    using System;
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
            throw new NotImplementedException("message");
            
            //var valuePath = ValueVisitor<TSource, TValue>.Create(path);
            //valuePath.CacheValue();
            //if (!valuePath.HasValue(source))
            //{
            //    return @default;
            //}
            //return valuePath.Value(source);
        }

        public static TValue ValueOrDefault<TValue>(Expression<Func<TValue>> path, TValue @default = default (TValue))
        {
            //var valuePath = ValueVisitor<object, TValue>.Create(path);
            //valuePath.CacheValue();
            //if (!valuePath.HasValue(source))
            //{
            //    return @default;
            //}
            //return valuePath.Value(source);
            throw new NotImplementedException("message");
        }

        public static IValuePath<TSource, TValue> ValuePath<TSource, TValue>(Expression<Func<TSource, TValue>> path)
        {
            throw new NotImplementedException();

            //var valuePath = Internals.ValuePath.CreatePropertyPath(path);
            //return valuePath;
        }

        public static IValuePath<TValue> ValuePath<TValue>(Expression<Func<TValue>> path)
        {
            throw new NotImplementedException();
            //var valuePath = Internals.ValuePath.CreatePropertyPath(path);
            //return valuePath;
        }
    }
}
