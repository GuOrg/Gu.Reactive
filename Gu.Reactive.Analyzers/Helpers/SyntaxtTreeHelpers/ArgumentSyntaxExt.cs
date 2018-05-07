namespace Gu.Reactive.Analyzers
{
    using System.Threading;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ArgumentSyntaxExt
    {
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
                if (method.Parameters.TryElementAt(index, out result))
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
    }
}
