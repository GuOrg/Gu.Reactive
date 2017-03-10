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

            return new SimpleFactory<TSource, TResult>(selector);
        }

        internal static IMappingFactory<TSource, TResult> Create<TSource, TResult>(Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater)
        {
            if (typeof(TSource).IsClass && typeof(TResult).IsClass)
            {
                return new MappingCache<TSource, TResult>(indexSelector, indexUpdater);
            }

            if (indexUpdater != null &&
                typeof(IDisposable).IsAssignableFrom(typeof(TResult)))
            {
                return new SimpleDisposingUpdatingFactory<TSource, TResult>(indexSelector, indexUpdater);
            }

            return new SimpleUpdatingFactory<TSource, TResult>(indexSelector, indexUpdater);
        }
    }
}