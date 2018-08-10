namespace Gu.Reactive.Analyzers.CodeFixes
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ObservableFromEventArgsFix))]
    [Shared]
    public class ObservableFromEventArgsFix : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(GUREA12ObservableFromEventDelegateType.DiagnosticId);

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

                var invocation = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<InvocationExpressionSyntax>();
                if (invocation != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Fix arguments.",
                            _ => ApplyUseSlimFixAsync(context, syntaxRoot, invocation),
                            nameof(ObservableFromEventArgsFix)),
                        diagnostic);
                    continue;
                }
            }
        }

        private static Task<Document> ApplyUseSlimFixAsync(CodeFixContext context, SyntaxNode syntaxRoot, InvocationExpressionSyntax invocation)
        {
            var arg0 = invocation.ArgumentList.Arguments[0];
            return Task.FromResult(
                context.Document.WithSyntaxRoot(
                    syntaxRoot.ReplaceNode(
                        invocation.ArgumentList,
                        invocation.ArgumentList.InsertNodesBefore(
                            arg0,
                            new[]
                            {
                                arg0.WithExpression(
                                        SyntaxFactory.ParseExpression("h => (_, e) => h(e)"))
                                    .WithLeadingTrivia(arg0.GetLeadingTrivia())
                                    .WithAdditionalAnnotations(Formatter.Annotation),
                            }))));
        }
    }
}
