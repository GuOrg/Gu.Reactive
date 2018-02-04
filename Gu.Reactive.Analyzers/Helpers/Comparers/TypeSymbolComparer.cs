namespace Gu.Reactive.Analyzers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    internal class TypeSymbolComparer : IEqualityComparer<ITypeSymbol>
    {
        internal static readonly TypeSymbolComparer Default = new TypeSymbolComparer();

        private TypeSymbolComparer()
        {
        }

        public static bool Equals(ITypeSymbol x, ITypeSymbol y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null ||
                y == null)
            {
                return false;
            }

            if (x is INamedTypeSymbol xNamed &&
                y is INamedTypeSymbol yNamed)
            {
                return NamedTypeSymbolComparer.Equals(xNamed, yNamed);
            }

            return x.MetadataName == y.MetadataName &&
                   x.ContainingNamespace?.MetadataName == y.ContainingNamespace?.MetadataName;
        }

        /// <inheritdoc />
        bool IEqualityComparer<ITypeSymbol>.Equals(ITypeSymbol x, ITypeSymbol y) => Equals(x, y);

        /// <inheritdoc />
        public int GetHashCode(ITypeSymbol obj)
        {
            return obj?.MetadataName.GetHashCode() ?? 0;
        }
    }
}