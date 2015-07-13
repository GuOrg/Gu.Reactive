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
    }
}