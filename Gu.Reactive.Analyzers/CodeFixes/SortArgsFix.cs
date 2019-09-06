namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Linq;
    using System.Threading.Tasks;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SortArgsFix))]
    [Shared]
    public class SortArgsFix : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            Descriptors.GUREA13SyncParametersAndArgs.Id);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNodeOrAncestor(diagnostic, out ConstructorInitializerSyntax initializer) &&
                    initializer.ArgumentList is ArgumentListSyntax argumentList &&
                    initializer.Parent is ConstructorDeclarationSyntax constructor &&
                    constructor.ParameterList is ParameterListSyntax parameterList)
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Sort arguments.",
                            _ => Task.FromResult(context.Document.WithSyntaxRoot(syntaxRoot.ReplaceNode(argumentList, Sort()))),
                            nameof(SortArgsFix)),
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
