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
            if (typeof(T) == typeof(IEventSymbol))
            {
                return (IEqualityComparer<T>)EventSymbolComparer.Default;
            }

            if (typeof(T) == typeof(IFieldSymbol))
            {
                return (IEqualityComparer<T>)FieldSymbolComparer.Default;
            }

            if (typeof(T) == typeof(ILocalSymbol))
            {
                return (IEqualityComparer<T>)LocalSymbolComparer.Default;
            }

            if (typeof(T) == typeof(IMethodSymbol))
            {
                return (IEqualityComparer<T>)MethodSymbolComparer.Default;
            }

            if (typeof(T) == typeof(INamedTypeSymbol))
            {
                return (IEqualityComparer<T>)NamedTypeSymbolComparer.Default;
            }

            if (typeof(T) == typeof(IParameterSymbol))
            {
                return (IEqualityComparer<T>)ParameterSymbolComparer.Default;
            }

            if (typeof(T) == typeof(IPropertySymbol))
            {
                return (IEqualityComparer<T>)PropertySymbolComparer.Default;
            }

            if (typeof(T) == typeof(ISymbol))
            {
                return (IEqualityComparer<T>)SymbolComparer.Default;
            }

            if (typeof(T) == typeof(ITypeSymbol))
            {
                return (IEqualityComparer<T>)TypeSymbolComparer.Default;
            }

            return EqualityComparer<T>.Default;
        }
    }
}
