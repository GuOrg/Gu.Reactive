namespace Gu.Reactive
{
    using System;

    internal static class Mapper
    {
        internal static IMapper<TSource, TResult> Create<TSource, TResult>(Func<TSource, TResult> selector)
        {
            if (!typeof(TSource).IsValueType &&
                !typeof(TResult).IsValueType)
            {
                return CreatingCaching(selector);
            }

            return new Creating<TSource, TResult>(selector);
        }

        private static IMapper<TSource, TResult> CreatingCaching<TSource, TResult>(Func<TSource, TResult> selector)
        {
            return (IMapper<TSource, TResult>)Activator.CreateInstance(
                typeof(CreatingCaching<,>).MakeGenericType(typeof(TSource), typeof(TResult)),
                new object[] { selector });
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

        //internal static IMapper<TSource, TResult> Create<TSource, TResult>(
        //    Func<TSource, int, TResult> indexSelector,
        //    Func<TResult, int, TResult> indexUpdater,
        //    Action<TResult> onRemove)
        //    where TSource : class
        //    where TResult : class
        //{
        //    return new UpdatingRemoving<TSource, TResult>(indexSelector, indexUpdater, onRemove);
        //}
    }
}