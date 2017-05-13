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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseSlimCodeFix))]
    [Shared]
    public class UseSlimCodeFix : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(GUREA04PreferSlim.DiagnosticId);

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

                var name = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<IdentifierNameSyntax>();
                if (name != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Use slim.",
                            _ => ApplyUseSlimFixAsync(context, syntaxRoot, name),
                            nameof(UseSlimCodeFix)),
                        diagnostic);
                    continue;
                }
            }
        }

        private static Task<Document> ApplyUseSlimFixAsync(CodeFixContext context, SyntaxNode syntaxRoot, IdentifierNameSyntax name)
        {
            var memberAccess = name.Parent as MemberAccessExpressionSyntax;
            return Task.FromResult(
                context.Document.WithSyntaxRoot(
                    syntaxRoot.ReplaceNode(
                        memberAccess,
                        memberAccess.WithName(
                            SyntaxFactory.IdentifierName("ObservePropertyChangedSlim")))));
        }
    }
}
