namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SortParametersFix))]
    [Shared]
    internal class SortParametersFix : DocumentEditorCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            Descriptors.GUREA13SyncParametersAndArgs.Id);

        protected override DocumentEditorFixAllProvider? FixAllProvider() => DocumentEditorFixAllProvider.Solution;

        protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot is { } &&
                    syntaxRoot.TryFindNodeOrAncestor(diagnostic, out ConstructorInitializerSyntax? initializer) &&
                    initializer.ArgumentList is { } argumentList &&
                    initializer.Parent is ConstructorDeclarationSyntax { ParameterList: ParameterListSyntax parameterList })
                {
                    context.RegisterCodeFix(
                        "Sort parameters.",
                        e => e.ReplaceNode(parameterList, Sort()),
                        nameof(SortParametersFix),
                        diagnostic);

                    ParameterListSyntax Sort()
                    {
                        return parameterList.WithParameters(
                            SyntaxFactory.SeparatedList(
                                parameterList.Parameters.OrderBy(x => IndexOf(x)),
                                parameterList.Parameters.GetSeparators()));

                        int IndexOf(ParameterSyntax parameter)
                        {
                            return argumentList.Arguments.IndexOf(x => ((IdentifierNameSyntax)x.Expression).Identifier.ValueText == parameter.Identifier.ValueText);
                        }
                    }
                }
            }
        }
    }
}
