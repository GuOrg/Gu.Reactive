namespace Gu.Reactive.Analyzers
{
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class PropertySymbolExt
    {
        internal static bool IsGetOnly(this IPropertySymbol property)
        {
            if (property.ContainingType == KnownSymbol.Nullable ||
                property.ContainingType == KnownSymbol.Type ||
                property.ContainingType == KnownSymbol.TimeSpan ||
                property.ContainingType == KnownSymbol.DateTime ||
                property.ContainingType == KnownSymbol.DateTimeOffset ||
                property.ContainingType == KnownSymbol.String)
            {
                return true;
            }

            if (!TryGetDeclaration(property, out var declaration))
            {
                return false;
            }

            if (!TryGetGetter(declaration, out var getter) ||
                getter.Body != null)
            {
                return false;
            }

            return !TryGetSetter(declaration, out AccessorDeclarationSyntax _);
        }

        internal static bool IsPrivateSetAssignedInCtorOnly(this IPropertySymbol property, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (TryGetDeclaration(property, out var declaration) &&
                TryGetGetter(declaration, out var getter) &&
                getter.Body == null &&
                TryGetSetter(declaration, out var setter) &&
                setter.Body == null)
            {
                using (var walker = MutationWalker.For(property, semanticModel, cancellationToken))
                {
                    foreach (var mutation in walker)
                    {
                        if (mutation.FirstAncestor<ConstructorDeclarationSyntax>() == null)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private static bool TryGetGetter(BasePropertyDeclarationSyntax declaration, out AccessorDeclarationSyntax getter)
        {
            return declaration.TryGetGetter(out getter);
        }

        private static bool TryGetSetter(BasePropertyDeclarationSyntax declaration, out AccessorDeclarationSyntax setter)
        {
            return declaration.TryGetSetter(out setter);
        }

        private static bool TryGetDeclaration(IPropertySymbol property, out BasePropertyDeclarationSyntax declaration)
        {
            if (property.DeclaringSyntaxReferences.Length != 1)
            {
                declaration = null;
                return false;
            }

            declaration = (BasePropertyDeclarationSyntax)property.DeclaringSyntaxReferences[0].GetSyntax();
            return declaration != null;
        }
    }
}
