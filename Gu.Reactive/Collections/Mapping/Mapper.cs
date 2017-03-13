namespace Gu.Reactive
{
    using System;
    using System.Reflection;

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
        {
            var type = typeof(TSource).IsValueType
                ? typeof(CreatingRemoving<,>).MakeGenericType(typeof(TSource), typeof(TResult))
                : typeof(CreatingCachingRemoving<,>).MakeGenericType(typeof(TSource), typeof(TResult));

            var args = new object[] { selector, onRemove };
            var constructor = type.GetConstructor(
                bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance,
                binder: null,
                types: Type.GetTypeArray(args),
                modifiers: null);
            return (IMapper<TSource, TResult>)constructor.Invoke(args);
        }

        internal static IMapper<TSource, TResult> Create<TSource, TResult>(
            Func<TSource, int, TResult> indexSelector,
            Func<TResult, int, TResult> indexUpdater,
            Action<TResult> onRemove)
            where TResult : class
        {
            return new UpdatingRemoving<TSource, TResult>(indexSelector, indexUpdater, onRemove);
        }

        private static IMapper<TSource, TResult> CreatingCaching<TSource, TResult>(Func<TSource, TResult> selector)
        {
            return (IMapper<TSource, TResult>)Activator.CreateInstance(
                typeof(CreatingCaching<,>).MakeGenericType(typeof(TSource), typeof(TResult)),
                new object[] { selector });
        }
    }
}