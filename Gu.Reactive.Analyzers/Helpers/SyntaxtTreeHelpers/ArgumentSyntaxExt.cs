namespace Gu.Reactive.Analyzers
{
    using System.Threading;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ArgumentSyntaxExt
    {
        internal static bool TryGetSymbol<T>(this ArgumentSyntax argument, SemanticModel semanticModel, CancellationToken cancellationToken, out T result)
            where T : class, ISymbol
        {
            result = semanticModel.GetSymbolSafe(argument.Expression, cancellationToken) as T;

            return result != null;
        }

        internal static bool TryGetParameter(this ArgumentSyntax argument, SemanticModel semanticModel, CancellationToken cancellationToken, out IParameterSymbol result)
        {
            var invocation = (SyntaxNode)argument.FirstAncestor<InvocationExpressionSyntax>() ??
                                         argument.FirstAncestor<ConstructorInitializerSyntax>();
            var method = (IMethodSymbol)semanticModel.GetSymbolSafe(invocation, cancellationToken);
            result = null;
            if (argument == null ||
                method?.Parameters == null)
            {
                return false;
            }

            if (argument.NameColon == null)
            {
                var index = argument.FirstAncestorOrSelf<ArgumentListSyntax>()
                                    .Arguments.IndexOf(argument);
                if (method.Parameters.TryGetAtIndex(index, out result))
                {
                    return true;
                }

                var temp = method.Parameters[method.Parameters.Length - 1];
                if (temp.IsParams)
                {
                    result = temp;
                    return true;
                }

                return false;
            }

            foreach (var candidate in method.Parameters)
            {
                if (candidate.Name == argument.NameColon.Name.Identifier.ValueText)
                {
                    result = candidate;
                    return true;
                }
            }

            return false;
        }

        internal static bool TryGetStringValue(this ArgumentSyntax argument, SemanticModel semanticModel, CancellationToken cancellationToken, out string result)
        {
            result = null;
            if (argument?.Expression == null || semanticModel == null)
            {
                return false;
            }

            if (argument.Expression.IsKind(SyntaxKind.NullLiteralExpression))
            {
                return true;
            }

            if (argument.Expression.IsKind(SyntaxKind.StringLiteralExpression) ||
                argument.Expression.IsNameOf())
            {
                var cv = semanticModel.GetConstantValueSafe(argument.Expression, cancellationToken);
                if (cv.HasValue && cv.Value is string)
                {
                    result = (string)cv.Value;
                    return true;
                }
            }

            var symbolInfo = semanticModel.GetSymbolSafe(argument.Expression, cancellationToken);
            if (symbolInfo?.ContainingType?.Name == "String" &&
                symbolInfo.Name == "Empty")
            {
                result = string.Empty;
                return true;
            }

            return false;
        }

        internal static bool TryGetTypeofValue(this ArgumentSyntax argument, SemanticModel semanticModel, CancellationToken cancellationToken, out ITypeSymbol result)
        {
            result = null;
            if (argument?.Expression == null || semanticModel == null)
            {
                return false;
            }

            if (argument.Expression is TypeOfExpressionSyntax typeOf)
            {
                var typeSyntax = typeOf.Type;
                var typeInfo = semanticModel.GetTypeInfoSafe(typeSyntax, cancellationToken);
                result = typeInfo.Type;
                return result != null;
            }

            return false;
        }

        private static bool IsNameOf(this ExpressionSyntax expression)
        {
            return (expression as InvocationExpressionSyntax)?.IsNameOf() == true;
        }
    }
}