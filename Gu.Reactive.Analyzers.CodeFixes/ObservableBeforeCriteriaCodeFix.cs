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

            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken)
                                             .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
                if (string.IsNullOrEmpty(token.ValueText) ||
                    token.IsMissing)
                {
                    continue;
                }

                var initializer = syntaxRoot.FindNode(diagnostic.Location.SourceSpan)
                                            .FirstAncestorOrSelf<ConstructorInitializerSyntax>();
                if (initializer != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Move obvservable before criteria.",
                            cancellationToken => ApplyObservableBeforeCriteriaFixAsync(
                                cancellationToken, context, initializer),
                            nameof(ObservableBeforeCriteriaCodeFix)),
                        diagnostic);
                }
            }
        }

        private static async Task<Document> ApplyObservableBeforeCriteriaFixAsync(
                CancellationToken cancellationToken,
                CodeFixContext context,
                ConstructorInitializerSyntax initializer)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken)
                                             .ConfigureAwait(false);
            editor.ReplaceNode(
                initializer.ArgumentList.Arguments[0].Expression,
                initializer.ArgumentList.Arguments[1].Expression
                           .WithLeadingTrivia(initializer.ArgumentList.Arguments[0].Expression.GetLeadingTrivia()));
            editor.ReplaceNode(
                initializer.ArgumentList.Arguments[1].Expression,
                initializer.ArgumentList.Arguments[0].Expression
                           .WithLeadingTrivia(initializer.ArgumentList.Arguments[1].Expression.GetLeadingTrivia()));
            return editor.GetChangedDocument();
        }
    }
}
