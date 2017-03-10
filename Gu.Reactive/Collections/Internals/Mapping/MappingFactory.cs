namespace Gu.Reactive
{
    using System;

    internal static class MappingFactory
    {
        internal static IMappingFactory<TSource, TResult> Create<TSource, TResult>(Func<TSource, TResult> selector)
        {
            return new SimpleFactory<TSource, TResult>(selector);
        }

        internal static IMappingFactory<TSource, TResult> Create<TSource, TResult>(Func<TSource, int, TResult> selector)
        {
            return new SimpleUpdatingFactory<TSource, TResult>(selector, (result, index) => result);
        }

        internal static IMappingFactory<TSource, TResult> Create<TSource, TResult>(Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater)
        {
            return new SimpleUpdatingFactory<TSource, TResult>(indexSelector, indexUpdater);
        }

        internal static IMappingFactory<TSource, TResult> Create<TSource, TResult>(Func<TSource, TResult> selector, Action<TResult> onRemove)
        {
            return new TrackingUpdatingFactory<TSource, TResult>((source, index) => selector(source), (result, index) => result, onRemove);
        }

        internal static IMappingFactory<TSource, TResult> Create<TSource, TResult>(Func<TSource, int, TResult> selector, Action<TResult> onRemove)
        {
            return new TrackingUpdatingFactory<TSource, TResult>(selector, (result, index) => result, onRemove);
        }

        internal static IMappingFactory<TSource, TResult> Create<TSource, TResult>(Func<TSource, int, TResult> indexSelector, Func<TResult, int, TResult> indexUpdater, Action<TResult> onRemove)
        {
            return new TrackingUpdatingFactory<TSource, TResult>(indexSelector, indexUpdater, onRemove);
        }
    }
}