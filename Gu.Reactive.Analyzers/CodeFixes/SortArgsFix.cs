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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SortArgsFix))]
    [Shared]
    public class SortArgsFix : DocumentEditorCodeFixProvider
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
                if (syntaxRoot.TryFindNodeOrAncestor(diagnostic, out ConstructorInitializerSyntax? initializer) &&
                    initializer.ArgumentList is { } argumentList &&
                    initializer.Parent is ConstructorDeclarationSyntax { ParameterList: ParameterListSyntax parameterList })
                {
                    context.RegisterCodeFix(
                        "Sort arguments.",
                        e => e.ReplaceNode(argumentList, Sort()),
                        nameof(SortArgsFix),
                        diagnostic);

                    ArgumentListSyntax Sort()
                    {
                        return argumentList.WithArguments(
                            SyntaxFactory.SeparatedList(
                                argumentList.Arguments.OrderBy(x => IndexOf(x)),
                                argumentList.Arguments.GetSeparators()));

                        int IndexOf(ArgumentSyntax argument)
                        {
                            return parameterList.Parameters.IndexOf(x => x.Identifier.ValueText == ((IdentifierNameSyntax)argument.Expression).Identifier.ValueText);
                        }
                    }
                }
            }
        }
    }
}
