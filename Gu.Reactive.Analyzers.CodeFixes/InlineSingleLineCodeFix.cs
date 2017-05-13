namespace Gu.Reactive.Analyzers.CodeFixes
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InlineSingleLineCodeFix))]
    [Shared]
    public class InlineSingleLineCodeFix : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(GUREA08InlineSingleLine.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
                if (string.IsNullOrEmpty(token.ValueText) ||
                    token.IsMissing)
                {
                    continue;
                }

                var argument = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<ArgumentSyntax>();
                if (argument != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Inline.",
                            cancellationToken => ApplyInlineSingleLineAsync(cancellationToken, context, syntaxRoot, argument),
                            nameof(InlineSingleLineCodeFix)),
                        diagnostic);
                    continue;
                }
            }
        }

        private static async Task<Document> ApplyInlineSingleLineAsync(CancellationToken cancellationToken, CodeFixContext context, SyntaxNode syntaxRoot, ArgumentSyntax argument)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken)
                                             .ConfigureAwait(false);
            var semanticModel = editor.SemanticModel;
            var method = (IMethodSymbol)semanticModel.GetSymbolSafe(argument.Expression, cancellationToken);
            MethodDeclarationSyntax methodDeclaration = null;
            foreach (var reference in method.DeclaringSyntaxReferences)
            {
                methodDeclaration = (MethodDeclarationSyntax)reference.GetSyntax(cancellationToken);
            }

            if (methodDeclaration == null)
            {
                return context.Document;
            }

            var expression = methodDeclaration.ExpressionBody?.Expression ??
                                   ((ReturnStatementSyntax)methodDeclaration.Body.Statements[0]).Expression;
            editor.ReplaceNode(
                argument.Expression,
                expression.WithLeadingTrivia(argument.Expression.GetLeadingTrivia()));
            editor.RemoveNode(methodDeclaration);
            return editor.GetChangedDocument();
        }
    }
}