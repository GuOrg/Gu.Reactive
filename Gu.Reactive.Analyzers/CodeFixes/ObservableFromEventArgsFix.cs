namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Formatting;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ObservableFromEventArgsFix))]
    [Shared]
    internal class ObservableFromEventArgsFix : DocumentEditorCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            Descriptors.GUREA12ObservableFromEventDelegateType.Id);

        protected override DocumentEditorFixAllProvider? FixAllProvider() => DocumentEditorFixAllProvider.Solution;

        protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNodeOrAncestor(diagnostic, out InvocationExpressionSyntax? invocation))
                {
                    context.RegisterCodeFix(
                        "Fix arguments.",
                        e => e.ReplaceNode(
                            invocation.ArgumentList,
                            x => x.InsertNodesBefore(
                                x.Arguments[0],
                                new[]
                                {
                                    SyntaxFactory.Argument(SyntaxFactory.ParseExpression("h => (_, e) => h(e)"))
                                                 .WithLeadingTrivia(x.Arguments[0].GetLeadingTrivia())
                                                 .WithAdditionalAnnotations(Formatter.Annotation),
                                })),
                        nameof(ObservableFromEventArgsFix),
                        diagnostic);
                }
            }
        }
    }
}
