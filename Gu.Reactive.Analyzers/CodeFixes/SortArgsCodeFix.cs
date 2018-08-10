namespace Gu.Reactive.Analyzers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

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
                if (syntaxRoot.TryFindNodeOrAncestor(diagnostic, out ConstructorInitializerSyntax initializer))
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Sort arguments.",
                            cancellationToken => ApplySortArgsAsync(cancellationToken, context, initializer),
                            nameof(SortArgsCodeFix)),
                        diagnostic);
                    continue;
                }
            }
        }

        private static async Task<Document> ApplySortArgsAsync(CancellationToken cancellationToken, CodeFixContext context, ConstructorInitializerSyntax initializer)
        {
            var argumentList = initializer.ArgumentList;
            var arguments = new List<ArgumentSyntax>(argumentList.Arguments);
            var parameterList = initializer.FirstAncestor<ConstructorDeclarationSyntax>().ParameterList.Parameters;
            arguments.Sort((x, y) => IndexOf(x, parameterList).CompareTo(IndexOf(y, parameterList)));
            var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken)
                                             .ConfigureAwait(false);
            for (var i = 0; i < argumentList.Arguments.Count; i++)
            {
                editor.ReplaceNode(argumentList.Arguments[i].Expression, arguments[i].Expression);
            }

            return editor.GetChangedDocument();
        }

        private static int IndexOf(ArgumentSyntax argument, SeparatedSyntaxList<ParameterSyntax> arguments)
        {
            return arguments.IndexOf(x => x.Identifier.ValueText == ((IdentifierNameSyntax)argument.Expression).Identifier.ValueText);
        }
    }
}
