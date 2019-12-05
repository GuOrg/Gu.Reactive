namespace Gu.Reactive.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InjectNegatedCodeFix))]
    [Shared]
    public class InjectNegatedCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(Descriptors.GUREA07DoNotNegateCondition.Id);

        public override FixAllProvider? GetFixAllProvider() => null;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);

            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken)
                                             .ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNodeOrAncestor(diagnostic, out InvocationExpressionSyntax? invocation) &&
                    invocation.TryFirstAncestor(out ConstructorDeclarationSyntax? ctor) &&
                    invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    var condition = memberAccess.Expression;
                    var symbol = semanticModel.GetSymbolSafe(condition, context.CancellationToken);
                    if (symbol is IParameterSymbol parameter)
                    {
                        using (var pooled = IdentifierNameWalker.Borrow(ctor))
                        {
                            foreach (var name in pooled.IdentifierNames)
                            {
                                if (name.Identifier.ValueText == parameter.Name)
                                {
                                    if (!condition.Contains(name))
                                    {
                                        return;
                                    }
                                }
                            }
                        }

                        if (ctor.TryFindParameter(parameter.Name, out ParameterSyntax? parameterSyntax))
                        {
                            context.RegisterCodeFix(
                                CodeAction.Create(
                                    "Inject negated.",
                                    cancellationToken => ApplyInjectNegatedFixAsync(context, parameterSyntax, invocation, cancellationToken),
                                    nameof(InjectNegatedCodeFix)),
                                diagnostic);
                        }
                    }
                }
            }
        }

        private static async Task<Document> ApplyInjectNegatedFixAsync(CodeFixContext context, ParameterSyntax parameter, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken)
                                                     .ConfigureAwait(false);
            if (parameter.Type is GenericNameSyntax { TypeArgumentList: { Arguments: { Count: 1 } } } genericName &&
                genericName.Identifier.ValueText == "Negated" &&
                parameter.Identifier.ValueText.StartsWith("not", StringComparison.OrdinalIgnoreCase))
            {
                var name = parameter.Identifier.ValueText.Replace("not", string.Empty).ToFirstCharLower();
                editor.ReplaceNode(
                    parameter,
                    editor.Generator.ParameterDeclaration(
                              name,
                              genericName.TypeArgumentList.Arguments[0])
                          .WithLeadingTrivia(parameter.GetLeadingTrivia()));
                editor.ReplaceNode(
                    invocation,
                    editor.Generator.IdentifierName(name)
                          .WithLeadingTrivia(invocation.GetLeadingTrivia()));
            }
            else
            {
                var name = "not" + parameter.Identifier.ValueText.ToFirstCharUpper();
                editor.ReplaceNode(
                    parameter,
                    editor.Generator.ParameterDeclaration(
                              name,
                              SyntaxFactory.ParseTypeName($"Negated<{parameter.Type}>"))
                          .WithLeadingTrivia(parameter.GetLeadingTrivia()));
                editor.ReplaceNode(
                    invocation,
                    editor.Generator.IdentifierName(name)
                          .WithLeadingTrivia(invocation.GetLeadingTrivia()));
            }

            return editor.GetChangedDocument();
        }
    }
}
