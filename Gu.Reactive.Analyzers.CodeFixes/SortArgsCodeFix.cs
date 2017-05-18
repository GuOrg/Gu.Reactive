namespace Gu.Reactive.Analyzers.CodeFixes
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SortArgsCodeFix))]
    [Shared]
    public class SortArgsCodeFix : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(GUREA13SyncParametersAndArgs.DiagnosticId);

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

                var initializer = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<ConstructorInitializerSyntax>();
                if (initializer != null)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Sort arguments.",
                            _ => ApplySortArgsAsync(context, syntaxRoot, initializer),
                            nameof(SortArgsCodeFix)),
                        diagnostic);
                    continue;
                }
            }
        }

        private static Task<Document> ApplySortArgsAsync(CodeFixContext context, SyntaxNode syntaxRoot, ConstructorInitializerSyntax initializer)
        {
            var argumentList = initializer.ArgumentList;
            var arguments = new List<ArgumentSyntax>(argumentList.Arguments);
            var parameterList = initializer.FirstAncestor<ConstructorDeclarationSyntax>().ParameterList.Parameters;
            arguments.Sort((x, y) => IndexOf(x, parameterList).CompareTo(IndexOf(y, parameterList)));
            return Task.FromResult(
                context.Document.WithSyntaxRoot(
                    syntaxRoot.ReplaceNode(
                        argumentList,
                        argumentList.WithArguments(SyntaxFactory.SeparatedList(arguments)))));
        }

        private static int IndexOf(ArgumentSyntax argument, SeparatedSyntaxList<ParameterSyntax> arguments)
        {
            return arguments.IndexOf(x => x.Identifier.ValueText == ((IdentifierNameSyntax)argument.Expression).Identifier.ValueText);
        }
    }
}