namespace Gu.Reactive
{
    using System;

    internal static class MappingFactory
    {
        internal static IMapper<TSource, TResult> Create<TSource, TResult>(Func<TSource, TResult> selector)
        {
            return new Creating<TSource, TResult>(selector);
        }

        internal static IMapper<TSource, TResult> Create<TSource, TResult>(
            Func<TSource, int, TResult> indexSelector, 
            Func<TResult, int, TResult> indexUpdater)
        {
            return new Updating<TSource, TResult>(indexSelector, indexUpdater);
        }

        internal static IMapper<TSource, TResult> Create<TSource, TResult>(
            Func<TSource, TResult> selector,
            Action<TResult> onRemove) 
            where TResult : class 
            where TSource : class
        {
            return new CreatingRemoving<TSource, TResult>(selector, onRemove);
        }

        internal static IMapper<TSource, TResult> Create<TSource, TResult>(
            Func<TSource, int, TResult> indexSelector, 
            Func<TResult, int, TResult> indexUpdater,
            Action<TResult> onRemove) 
            where TSource : class
            where TResult : class
        {
            return new UpdatingRemoving<TSource, TResult>(indexSelector, indexUpdater, onRemove);
        }
    }
}