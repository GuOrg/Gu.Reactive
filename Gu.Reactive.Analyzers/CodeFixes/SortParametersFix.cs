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

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SortParametersFix))]
    [Shared]
    public class SortParametersFix : CodeFixProvider
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
                            "Sort parameters.",
                            _ => Task.FromResult(context.Document.WithSyntaxRoot(syntaxRoot.ReplaceNode(parameterList, Sort()))),
                            nameof(SortParametersFix)),
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
