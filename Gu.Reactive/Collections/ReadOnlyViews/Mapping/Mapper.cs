namespace Gu.Reactive
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    internal static class Mapper
    {
        internal static IMapper<TSource, TResult> Create<TSource, TResult>(Func<TSource, TResult> selector)
        {
            if (typeof(TSource).IsClass &&
                typeof(TResult).IsClass)
            {
                return Factory<TSource, TResult>.CreatingCaching(selector);
            }

            return new Creating<TSource, TResult>(selector);
        }

        internal static IMapper<TSource, TResult> Create<TSource, TResult>(
            Func<TSource, int, TResult> selector,
            Func<TResult, int, TResult> updater)
        {
            return new Updating<TSource, TResult>(selector, updater);
        }

        internal static IMapper<TSource, TResult> Create<TSource, TResult>(
            Func<TSource, TResult> selector,
            Action<TResult> onRemove)
            where TResult : class
        {
            if (typeof(TSource).IsValueType)
            {
                return Factory<TSource, TResult>.CreatingRemoving(selector, onRemove);
            }

            return Factory<TSource, TResult>.CreatingCachingRemoving(selector, onRemove);
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
            where TSource : class
            where TResult : class
        {
            return new CreatingCaching<TSource, TResult>(selector);
        }

        private static IMapper<TSource, TResult> CreatingRemoving<TSource, TResult>(Func<TSource, TResult> selector, Action<TResult> onRemove)
            where TSource : struct
            where TResult : class
        {
            return new CreatingRemoving<TSource, TResult>(selector, onRemove);
        }

        private static IMapper<TSource, TResult> CreatingCachingRemoving<TSource, TResult>(Func<TSource, TResult> selector, Action<TResult> onRemove)
            where TSource : class
            where TResult : class
        {
            return new CreatingCachingRemoving<TSource, TResult>(selector, onRemove);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private static class Factory<TSource, TResult>
        {
            private static Func<Func<TSource, TResult>, IMapper<TSource, TResult>> creatingCaching;
            private static Func<Func<TSource, TResult>, Action<TResult>, IMapper<TSource, TResult>> creatingRemoving;
            private static Func<Func<TSource, TResult>, Action<TResult>, IMapper<TSource, TResult>> creatingCachingRemoving;

            internal static IMapper<TSource, TResult> CreatingCaching(Func<TSource, TResult> selector)
            {
                if (creatingCaching is null)
                {
                    creatingCaching = CreateDelegate<Func<Func<TSource, TResult>, IMapper<TSource, TResult>>>(nameof(Mapper.CreatingCaching));
                }

                return creatingCaching.Invoke(selector);
            }

            internal static IMapper<TSource, TResult> CreatingRemoving(Func<TSource, TResult> selector, Action<TResult> onRemove)
            {
                if (creatingRemoving is null)
                {
                    creatingRemoving = CreateDelegate<Func<Func<TSource, TResult>, Action<TResult>, IMapper<TSource, TResult>>>(nameof(Mapper.CreatingRemoving));
                }

                return creatingRemoving.Invoke(selector, onRemove);
            }

            internal static IMapper<TSource, TResult> CreatingCachingRemoving(Func<TSource, TResult> selector, Action<TResult> onRemove)
            {
                if (creatingCachingRemoving is null)
                {
                    creatingCachingRemoving = CreateDelegate<Func<Func<TSource, TResult>, Action<TResult>, IMapper<TSource, TResult>>>(nameof(Mapper.CreatingCachingRemoving));
                }

                return creatingCachingRemoving.Invoke(selector, onRemove);
            }

            private static T CreateDelegate<T>(string methodName)
                where T : Delegate
            {
                var method = typeof(Mapper)
                             .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
                             .MakeGenericMethod(typeof(TSource), typeof(TResult));
                return (T)Delegate.CreateDelegate(typeof(T), method);
            }
        }
    }
}
