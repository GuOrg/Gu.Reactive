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
    using Microsoft.CodeAnalysis.Simplification;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EventSubscriptionToObserveFix))]
    [Shared]
    public class EventSubscriptionToObserveFix : CodeFixProvider
    {
        private const string ObservableFromEventString = @"System.Reactive.Linq.Observable.FromEvent<HANDLERTYPE, ARGTYPE>(
                h => LEFT += h,
                h => LEFT -= h)
                                           .Subscribe(LAMBDA)";

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(GUREA11Observe.DiagnosticId);

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

                var assignment = syntaxRoot.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<AssignmentExpressionSyntax>();
                if (assignment.Right is ParenthesizedLambdaExpressionSyntax lambda)
                {
                    using (var pooled = IdentifierNameWalker.Create(lambda.Body))
                    {
                        bool usesArg = false;
                        foreach (var name in pooled.Item.IdentifierNames)
                        {
                            if (name.Identifier.ValueText == lambda.ParameterList.Parameters[0].Identifier.ValueText)
                            {
                                return;
                            }

                            if (name.Identifier.ValueText == lambda.ParameterList.Parameters[1].Identifier.ValueText)
                            {
                                usesArg = true;
                            }
                        }

                        context.RegisterCodeFix(
                            CodeAction.Create(
                                "Observe.Event",
                                cancellationToken => ApplyObserveEventFixAsync(cancellationToken, context, assignment, usesArg),
                                nameof(EventSubscriptionToObserveFix)),
                            diagnostic);
                    }
                }
            }
        }

        private static async Task<Document> ApplyObserveEventFixAsync(
            CancellationToken cancellationToken,
            CodeFixContext context,
            AssignmentExpressionSyntax assignment,
            bool usesArg)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken)
                                                     .ConfigureAwait(false);
            var eventSymbol = (IEventSymbol)editor.SemanticModel.GetSymbolSafe(assignment.Left, cancellationToken);
            var observeSubscribe = ObservableFromEventString.Replace("HANDLERTYPE", eventSymbol.Type.ToDisplayString())
                                                   .Replace("ARGTYPE", ArgType(eventSymbol))
                                                   .Replace("LEFT", assignment.Left.ToString())
                                                   .Replace("LAMBDA", Lambda((ParenthesizedLambdaExpressionSyntax)assignment.Right, usesArg));
            editor.ReplaceNode(
                assignment,
                SyntaxFactory.ParseExpression(observeSubscribe)
                             .WithLeadingTrivia(assignment.GetLeadingTrivia())
                             .WithAdditionalAnnotations(Simplifier.Annotation));
            return editor.GetChangedDocument();
        }

        private static string ArgType(IEventSymbol symbol)
        {
            if (symbol.Type == KnownSymbol.EventHandler)
            {
                return "EventArgs";
            }

            if (symbol.Type is INamedTypeSymbol namedType)
            {
                return namedType.TypeArguments[0]
                                .ToDisplayString();
            }

            return "UNKNOWN";
        }

        private static string Lambda(ParenthesizedLambdaExpressionSyntax old, bool usesArg)
        {
            if (usesArg)
            {
                return old.WithParameterList(SyntaxFactory.ParseParameterList($"{old.ParameterList.Parameters[1]} "))
                          .ToString();
            }

            return old.WithParameterList(SyntaxFactory.ParseParameterList("_ "))
                      .ToString();
        }
    }
}