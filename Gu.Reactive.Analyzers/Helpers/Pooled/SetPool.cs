namespace Gu.Reactive.Analyzers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    internal static class SetPool<T>
    {
        private static readonly Pool<HashSet<T>> Pool = new Pool<HashSet<T>>(
            () => new HashSet<T>(GetComparer()),
            x => x.Clear());

        public static Pool<HashSet<T>>.Pooled Create()
        {
            return Pool.GetOrCreate();
        }

        private static IEqualityComparer<T> GetComparer()
        {
            if (typeof(T) == typeof(IPropertySymbol))
            {
                return (IEqualityComparer<T>)PropertySymbolComparer.Default;
            }

            return EqualityComparer<T>.Default;
        }
    }
}
