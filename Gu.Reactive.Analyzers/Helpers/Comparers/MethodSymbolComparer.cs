namespace Gu.Reactive.Analyzers
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;

    internal class MethodSymbolComparer : IEqualityComparer<IMethodSymbol>
    {
        internal static readonly MethodSymbolComparer Default = new MethodSymbolComparer();

        private MethodSymbolComparer()
        {
        }

        public static bool Equals(IMethodSymbol x, IMethodSymbol y)
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

            return x.MetadataName == y.MetadataName &&
                   NamedTypeSymbolComparer.Equals(x.ContainingType, y.ContainingType) &&
                   ParametersMatches(x, y);
        }

        /// <inheritdoc />
        bool IEqualityComparer<IMethodSymbol>.Equals(IMethodSymbol x, IMethodSymbol y) => Equals(x, y);

        /// <inheritdoc />
        public int GetHashCode(IMethodSymbol obj)
        {
            return obj?.MetadataName.GetHashCode() ?? 0;
        }

        private static bool ParametersMatches(IMethodSymbol x, IMethodSymbol y)
        {
            if (x.Parameters.Length != y.Parameters.Length)
            {
                return false;
            }

            for (var i = 0; i < x.Parameters.Length; i++)
            {
                if (!TypeSymbolComparer.Equals(x.Parameters[i].Type, y.Parameters[i].Type))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
