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
    public class ObservableFromEventArgsFix : DocumentEditorCodeFixProvider
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
                                    Argument()
                                     .WithLeadingTrivia(x.Arguments[0].GetLeadingTrivia())
                                     .WithAdditionalAnnotations(Formatter.Annotation),
                                })),
                        nameof(ObservableFromEventArgsFix),
                        diagnostic);

                    ArgumentSyntax Argument()
                    {
                        return SyntaxFactory.Argument(
                            SyntaxFactory.SimpleLambdaExpression(
                                parameter: SyntaxFactory.Parameter(SyntaxFactory.Identifier("h")),
                                body: SyntaxFactory.ParenthesizedLambdaExpression(
                                    parameterList: SyntaxFactory.ParameterList(
                                        parameters: SyntaxFactory.SeparatedList(
                                            new[]
                                            {
                                                SyntaxFactory.Parameter(identifier: SyntaxFactory.Identifier(text: "_")),
                                                SyntaxFactory.Parameter(identifier: SyntaxFactory.Identifier(text: "e")),
                                            },
                                            new[]
                                            {
                                                SyntaxFactory.Token(kind: SyntaxKind.CommaToken),
                                            })),
                                    body: SyntaxFactory.InvocationExpression(
                                        expression: SyntaxFactory.IdentifierName("h"),
                                        argumentList: SyntaxFactory.ArgumentList(
                                            arguments: SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName("e"))))))));
                    }
                }
            }
        }
    }
}
