namespace Gu.Reactive
{
    using System;

    internal static class MappingFactory
    {
        internal static IMappingFactory<TSource, TResult> Create<TSource, TResult>(Func<TSource, TResult> selector)
        {
            if (typeof(TSource).IsClass && typeof(TResult).IsClass)
            {
                return new MappingCache<TSource, TResult>(selector);
            }

            return new MappingFactory<TSource, TResult>(selector);
        }

        public static IMappingFactory<TSource, TResult> Create<TSource, TResult>(Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater)
        {
            if (typeof(TSource).IsClass && typeof(TResult).IsClass)
            {
                return new MappingCache<TSource, TResult>(indexSelector, indexUpdater);
            }

            return new MappingFactory<TSource, TResult>(indexSelector); // no caching so indexUpdater is omitted
        }
    }
}