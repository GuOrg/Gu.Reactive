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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ObservableBeforeCriteriaCodeFix))]
    [Shared]
    public class ObservableBeforeCriteriaCodeFix : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(GUREA09ObservableBeforeCriteria.DiagnosticId);

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

                var argumentList = syntaxRoot.FindNode(diagnostic.Location.SourceSpan)
                                            .FirstAncestorOrSelf<ArgumentListSyntax>();
                if (argumentList != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Move observable before criteria.",
                            cancellationToken => ApplyObservableBeforeCriteriaFixAsync(
                                cancellationToken, context, argumentList),
                            nameof(ObservableBeforeCriteriaCodeFix)),
                        diagnostic);
                }
            }
        }

        private static async Task<Document> ApplyObservableBeforeCriteriaFixAsync(
                CancellationToken cancellationToken,
                CodeFixContext context,
                ArgumentListSyntax argumentList)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken)
                                             .ConfigureAwait(false);
            editor.ReplaceNode(
                argumentList.Arguments[0].Expression,
                argumentList.Arguments[1].Expression
                           .WithLeadingTrivia(argumentList.Arguments[0].Expression.GetLeadingTrivia()));
            editor.ReplaceNode(
                argumentList.Arguments[1].Expression,
                argumentList.Arguments[0].Expression
                           .WithLeadingTrivia(argumentList.Arguments[1].Expression.GetLeadingTrivia()));
            return editor.GetChangedDocument();
        }
    }
}
