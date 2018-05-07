namespace Gu.Reactive.Analyzers.CodeFixes
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SortParametersCodeFix))]
    [Shared]
    public class SortParametersCodeFix : CodeFixProvider
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
                            "Sort parameters.",
                            cancellationToken => ApplySortParametersAsync(cancellationToken, context, initializer),
                            nameof(SortParametersCodeFix)),
                        diagnostic);
                    continue;
                }
            }
        }

        private static async Task<Document> ApplySortParametersAsync(CancellationToken cancellationToken, CodeFixContext context, ConstructorInitializerSyntax initializer)
        {
            var parameterList = initializer.FirstAncestor<ConstructorDeclarationSyntax>().ParameterList;
            var parameters = new List<ParameterSyntax>(parameterList.Parameters);
            var arguments = initializer.ArgumentList.Arguments;
            parameters.Sort((x, y) => IndexOf(x, arguments).CompareTo(IndexOf(y, arguments)));
            var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken)
                                             .ConfigureAwait(false);
            for (var i = 0; i < parameterList.Parameters.Count; i++)
            {
                editor.ReplaceNode(parameterList.Parameters[i], parameters[i]);
            }

            return editor.GetChangedDocument();
        }

        private static int IndexOf(ParameterSyntax parameter, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return arguments.IndexOf(x => ((IdentifierNameSyntax)x.Expression).Identifier.ValueText == parameter.Identifier.ValueText);
        }
    }
}
