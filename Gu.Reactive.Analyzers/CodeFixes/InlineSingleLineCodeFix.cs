namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InlineSingleLineCodeFix))]
    [Shared]
    public class InlineSingleLineCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(Descriptors.GUREA08InlineSingleLine.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNodeOrAncestor(diagnostic, out InvocationExpressionSyntax? invocation))
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Inline.",
                            cancellationToken => ApplyInlineSingleLineAsync(context, invocation, cancellationToken),
                            nameof(InlineSingleLineCodeFix)),
                        diagnostic);
                    continue;
                }
            }
        }

        private static async Task<Document> ApplyInlineSingleLineAsync(CodeFixContext context, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken)
                                             .ConfigureAwait(false);
            var semanticModel = editor.SemanticModel;
            var method = (IMethodSymbol)semanticModel.GetSymbolSafe(invocation.Expression, cancellationToken);
            var methodDeclaration = (MethodDeclarationSyntax)method.DeclaringSyntaxReferences[0].GetSyntax(cancellationToken);
            var expression = methodDeclaration.ExpressionBody?.Expression ??
                             ((ReturnStatementSyntax)methodDeclaration.Body.Statements[0]).Expression;

            if (invocation.ArgumentList?.Arguments.Count == 1 &&
                semanticModel.GetSymbolSafe(invocation.ArgumentList.Arguments[0].Expression, cancellationToken) is IParameterSymbol parameter)
            {
                expression = Rename.Parameter(method.Parameters[0], parameter, expression, semanticModel, cancellationToken);
                editor.ReplaceNode(
                    invocation,
                    expression.WithLeadingTrivia(invocation.Expression.GetLeadingTrivia()));
                editor.RemoveNode(methodDeclaration);
                return editor.GetChangedDocument();
            }

            return editor.OriginalDocument;
        }

        private class Rename : CSharpSyntaxRewriter
        {
            private readonly IParameterSymbol @from;
            private readonly IParameterSymbol to;
            private readonly SemanticModel semanticModel;
            private readonly CancellationToken cancellationToken;

            private Rename(IParameterSymbol from, IParameterSymbol to, SemanticModel semanticModel, CancellationToken cancellationToken)
            {
                this.@from = @from;
                this.to = to;
                this.semanticModel = semanticModel;
                this.cancellationToken = cancellationToken;
            }

            public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
            {
                if (node.Identifier.ValueText == this.from.Name)
                {
                    if (SymbolComparer.Equals(this.semanticModel.GetSymbolSafe(node, this.cancellationToken), this.from))
                    {
                        return SyntaxFactory.IdentifierName(this.to.Name);
                    }
                }

                return base.VisitIdentifierName(node);
            }

            internal static T Parameter<T>(
                IParameterSymbol from,
                IParameterSymbol to,
                T node,
                SemanticModel semanticModel,
                CancellationToken cancellationToken)
                where T : SyntaxNode
            {
                if (from.Name == to.Name)
                {
                    return node;
                }

                var rename = new Rename(@from, to, semanticModel, cancellationToken);
                return (T)rename.Visit(node);
            }
        }
    }
}
