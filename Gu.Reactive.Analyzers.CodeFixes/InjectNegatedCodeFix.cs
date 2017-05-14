namespace Gu.Reactive.Analyzers.CodeFixes
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
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
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(GUREA07DontNegateCondition.DiagnosticId);

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);

            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                var token = syntaxRoot.FindToken(diagnostic.Location.SourceSpan.Start);
                if (string.IsNullOrEmpty(token.ValueText) ||
                    token.IsMissing)
                {
                    continue;
                }

                var ctor = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<ConstructorDeclarationSyntax>();
                var invocation = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<InvocationExpressionSyntax>();

                if (ctor != null &&
                    invocation != null &&
                    invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    var condition = memberAccess.Expression;
                    var symbol = semanticModel.GetSymbolSafe(condition, context.CancellationToken);
                    if (symbol is IParameterSymbol parameter)
                    {
                        using (var pooled = IdentifierNameWalker.Create(ctor))
                        {
                            foreach (var name in pooled.Item.IdentifierNames)
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

                        if (ctor.ParameterList.Parameters.TryGetFirst(p => p.Identifier.ValueText == parameter.Name, out ParameterSyntax parameterSyntax))
                        {
                            context.RegisterCodeFix(
                                CodeAction.Create(
                                    "Inject negated.",
                                    cancellationToken => ApplyInjectNegatedFixAsync(cancellationToken, context, parameterSyntax, invocation),
                                    nameof(InjectNegatedCodeFix)),
                                diagnostic);
                            continue;
                        }
                    }
                }
            }
        }

        private static async Task<Document> ApplyInjectNegatedFixAsync(
            CancellationToken cancellationToken,
            CodeFixContext context,
            ParameterSyntax parameter,
            InvocationExpressionSyntax invocation)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken)
                                                     .ConfigureAwait(false);
            if (parameter.Type is GenericNameSyntax genericName &&
                genericName.Identifier.ValueText == "Negated" &&
                genericName.TypeArgumentList?.Arguments.Count == 1 &&
                parameter.Identifier.ValueText.StartsWith("not"))
            {
                var name = parameter.Identifier.ValueText.Replace("not", string.Empty).FirstCharLower();
                editor.ReplaceNode(
                    parameter,
                    editor.Generator.ParameterDeclaration(
                        name,
                        genericName.TypeArgumentList.Arguments[0]));
                editor.ReplaceNode(invocation, editor.Generator.IdentifierName(name));
            }
            else
            {
                var name = "not" + parameter.Identifier.ValueText.FirstCharUpper();
                editor.ReplaceNode(
                    parameter,
                    editor.Generator.ParameterDeclaration(
                        name,
                        SyntaxFactory.ParseTypeName($"Negated<{parameter.Type}>")));
                editor.ReplaceNode(invocation, editor.Generator.IdentifierName(name));
            }

            return editor.GetChangedDocument();
        }
    }
}
