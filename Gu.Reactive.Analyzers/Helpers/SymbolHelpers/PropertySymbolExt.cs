namespace Gu.Reactive.Analyzers
{
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class PropertySymbolExt
    {
        internal static bool IsPrivateSetAssignedInCtorOnly(this IPropertySymbol property, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (TryGetDeclaration(property, out var declaration) &&
                declaration.TryGetGetter(out var getter) &&
                getter.Body == null &&
                getter.ExpressionBody == null &&
                declaration.TryGetSetter(out var setter) &&
                setter.Body == null &&
                setter.ExpressionBody == null)
            {
                using (var walker = MutationWalker.For(property, semanticModel, cancellationToken))
                {
                    foreach (var mutation in walker.All())
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
