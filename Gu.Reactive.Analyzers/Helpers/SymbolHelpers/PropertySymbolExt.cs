namespace Gu.Reactive.Analyzers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class PropertySymbolExt
    {
        internal static bool IsPrivateSetAssignedInCtorOnly(this IPropertySymbol property, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (TryGetDeclaration(property, out var declaration))
            {
                if (declaration.TryGetGetter(out var getter) &&
                    getter is { Body: null, ExpressionBody: null } &&
                    declaration.TryGetSetter(out var setter) &&
                    setter is { Body: null, ExpressionBody: null })
                {
                    using var walker = MutationWalker.For(property, semanticModel, cancellationToken);
                    foreach (var mutation in walker.All())
                    {
                        if (mutation.FirstAncestor<ConstructorDeclarationSyntax>() == null)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }

            return property.SetMethod == null;
        }

        private static bool TryGetDeclaration(IPropertySymbol property, [NotNullWhen(true)] out BasePropertyDeclarationSyntax? declaration)
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
