namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ObservableBeforeCriteriaCodeFix))]
    [Shared]
    internal class ObservableBeforeCriteriaCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            Descriptors.GUREA09ObservableBeforeCriteria.Id);

        public override FixAllProvider? GetFixAllProvider() => null;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot is { } &&
                    syntaxRoot.TryFindNodeOrAncestor(diagnostic, out ArgumentListSyntax? argumentList))
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Move observable before criteria.",
                            cancellationToken => ApplyObservableBeforeCriteriaFixAsync(context, argumentList, cancellationToken),
                            nameof(ObservableBeforeCriteriaCodeFix)),
                        diagnostic);
                }
            }
        }

        private static async Task<Document> ApplyObservableBeforeCriteriaFixAsync(CodeFixContext context, ArgumentListSyntax argumentList, CancellationToken cancellationToken)
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
