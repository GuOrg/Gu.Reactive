namespace Gu.Reactive.Analyzers
{
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class Property
    {
        internal static bool AssignsSymbolInSetter(IPropertySymbol property, ISymbol symbol, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var setMethod = property?.SetMethod;
            if (setMethod == null ||
                setMethod.DeclaringSyntaxReferences.Length == 0)
            {
                return false;
            }

            if (TryGetSetter(property, cancellationToken, out AccessorDeclarationSyntax setter))
            {
                if (AssignmentExecutionWalker.FirstFor(symbol, setter, Scope.Instance, semanticModel, cancellationToken, out _))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool TryGetSetter(this IPropertySymbol property, CancellationToken cancellationToken, out AccessorDeclarationSyntax setter)
        {
            setter = null;
            if (property == null)
            {
                return false;
            }

            foreach (var reference in property.DeclaringSyntaxReferences)
            {
                var propertyDeclaration = reference.GetSyntax(cancellationToken) as PropertyDeclarationSyntax;
                if (propertyDeclaration.TryGetSetter(out setter))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
