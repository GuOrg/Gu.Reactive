// ReSharper disable UnusedMember.Global
namespace Gu.Reactive.Analyzers
{
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class ExpressionSyntaxExt
    {
        internal static bool Is(this ExpressionSyntax expression, string metadataName, SyntaxNodeAnalysisContext context)
        {
            return expression.Is(metadataName, context.SemanticModel, context.CancellationToken);
        }

        internal static bool Is(this ExpressionSyntax expression, string metadataName, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var type = semanticModel?.Compilation.GetTypeByMetadataName(metadataName);
            return expression.Is(type, semanticModel, cancellationToken);
        }

        internal static bool Is(this ExpressionSyntax expression, ITypeSymbol type, SyntaxNodeAnalysisContext context)
        {
            return expression.Is(type, context.SemanticModel, context.CancellationToken);
        }

        internal static bool Is(this ExpressionSyntax expression, ITypeSymbol type, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (expression == null || type == null)
            {
                return false;
            }

            var symbol = SemanticModelExt.GetTypeInfoSafe(semanticModel, expression, cancellationToken)
                                      .Type;
            return TypeSymbolExt.Is(symbol, type);
        }

        internal static bool IsSameType(this ExpressionSyntax expression, QualifiedType metadataName, SyntaxNodeAnalysisContext context)
        {
            return expression.IsSameType(metadataName, context.SemanticModel, context.CancellationToken);
        }

        internal static bool IsSameType(this ExpressionSyntax expression, QualifiedType metadataName, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var type = semanticModel?.Compilation.GetTypeByMetadataName(metadataName.FullName);
            return expression.IsSameType(type, semanticModel, cancellationToken);
        }

        internal static bool IsSameType(this ExpressionSyntax expression, ITypeSymbol type, SyntaxNodeAnalysisContext context)
        {
            return expression.IsSameType(type, context.SemanticModel, context.CancellationToken);
        }

        internal static bool IsSameType(this ExpressionSyntax expression, ITypeSymbol type, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            if (expression == null || type == null)
            {
                return false;
            }

            var symbol = SemanticModelExt.GetTypeInfoSafe(semanticModel, expression, cancellationToken)
                                      .Type;
            return TypeSymbolComparer.Equals(symbol, type);
        }
    }
}
