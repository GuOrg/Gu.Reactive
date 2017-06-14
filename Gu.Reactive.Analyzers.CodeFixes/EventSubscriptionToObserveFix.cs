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

        private const string ObservableFromEventWithConvertString = @"System.Reactive.Linq.Observable.FromEvent<HANDLERTYPE, ARGTYPE>(
                h => (_, e) => h(e),
                h => LEFT += h,
                h => LEFT -= h)
                                           .Subscribe(LAMBDA)";

        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(GUREA11PreferObservableFromEvent.DiagnosticId);

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

                var assignment = syntaxRoot.FindNode(diagnostic.Location.SourceSpan)
                                           .FirstAncestorOrSelf<AssignmentExpressionSyntax>();
                if (assignment == null ||
                    assignment.Left == null ||
                    assignment.Right == null)
                {
                    continue;
                }

                if (assignment.Right is ParenthesizedLambdaExpressionSyntax lambda &&
                    lambda.Body != null)
                {
                    using (var pooled = IdentifierNameWalker.Create(lambda.Body))
                    {
                        var usesArg = false;
                        foreach (var name in pooled.Item.IdentifierNames)
                        {
                            if (name.Identifier.ValueText == lambda.ParameterList.Parameters[0]
                                                                   .Identifier.ValueText)
                            {
                                return;
                            }

                            if (name.Identifier.ValueText == lambda.ParameterList.Parameters[1]
                                                                   .Identifier.ValueText)
                            {
                                usesArg = true;
                            }
                        }

                        context.RegisterCodeFix(
                            CodeAction.Create(
                                "Observe.Event",
                                cancellationToken => ApplyObserveEventLamdaFixAsync(
                                    cancellationToken, context, assignment, usesArg),
                                nameof(EventSubscriptionToObserveFix)),
                            diagnostic);
                    }
                }

                if (assignment.Right is MemberAccessExpressionSyntax memberAccess)
                {
                    var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
                    if (semanticModel.GetSymbolSafe(memberAccess, context.CancellationToken) is IMethodSymbol method &&
                        method.DeclaredAccessibility == Accessibility.Private &&
                        method.DeclaringSyntaxReferences.Length == 1)
                    {
                        var methodDeclaration = (MethodDeclarationSyntax)method.DeclaringSyntaxReferences[0].GetSyntax(context.CancellationToken);
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                "Observe.Event",
                                cancellationToken => ApplyObserveEventMethodFixAsync(cancellationToken, context, assignment, methodDeclaration),
                                nameof(EventSubscriptionToObserveFix)),
                            diagnostic);
                    }
                }
            }
        }

        private static async Task<Document> ApplyObserveEventLamdaFixAsync(
            CancellationToken cancellationToken,
            CodeFixContext context,
            AssignmentExpressionSyntax assignment,
            bool usesArg)
        {
            var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken)
                                                     .ConfigureAwait(false);
            var eventSymbol = (IEventSymbol)editor.SemanticModel.GetSymbolSafe(assignment.Left, cancellationToken);
            var observeSubscribe = GetObservableFromEventString(eventSymbol)
                .Replace("HANDLERTYPE", eventSymbol.Type.ToDisplayString())
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

        private static async Task<Document> ApplyObserveEventMethodFixAsync(
            CancellationToken cancellationToken,
            CodeFixContext context,
            AssignmentExpressionSyntax assignment,
            MethodDeclarationSyntax methodDeclaration)
        {
            var usesArg = false;
            if (methodDeclaration.ParameterList.Parameters.Any())
            {
                using (var pooled = IdentifierNameWalker.Create((SyntaxNode)methodDeclaration.Body ?? methodDeclaration.ExpressionBody))
                {
                    foreach (var name in pooled.Item.IdentifierNames)
                    {
                        if (name.Identifier.ValueText == methodDeclaration.ParameterList.Parameters[0].Identifier.ValueText)
                        {
                            if (methodDeclaration.ParameterList.Parameters.Count == 1)
                            {
                                usesArg = true;
                                continue;
                            }

                            return context.Document;
                        }

                        if (methodDeclaration.ParameterList.Parameters.Count == 2 &&
                            name.Identifier.ValueText == methodDeclaration.ParameterList.Parameters[1]
                                                                          .Identifier.ValueText)
                        {
                            usesArg = true;
                        }
                    }
                }
            }

            var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken)
                                             .ConfigureAwait(false);

            var eventSymbol = (IEventSymbol)editor.SemanticModel.GetSymbolSafe(assignment.Left, cancellationToken);
            var observeSubscribe = GetObservableFromEventString(eventSymbol)
                .Replace("HANDLERTYPE", eventSymbol.Type.ToDisplayString())
                .Replace("ARGTYPE", ArgType(eventSymbol))
                .Replace("LEFT", assignment.Left.ToString())
                .Replace("LAMBDA", Lambda(methodDeclaration, usesArg));
            editor.ReplaceNode(
                assignment,
                SyntaxFactory.ParseExpression(observeSubscribe)
                             .WithLeadingTrivia(assignment.GetLeadingTrivia())
                             .WithAdditionalAnnotations(Simplifier.Annotation));
            if (methodDeclaration.ParameterList.Parameters.Count == 2)
            {
                editor.RemoveNode(methodDeclaration.ParameterList.Parameters[0]);
            }

            if (!usesArg &&
                methodDeclaration.ParameterList.Parameters.Any())
            {
                editor.RemoveNode(methodDeclaration.ParameterList.Parameters.Last());
            }

            return editor.GetChangedDocument();
        }

        private static string GetObservableFromEventString(IEventSymbol eventSymbol)
        {
            if (eventSymbol.Type is INamedTypeSymbol namedTypeSymbol &&
                namedTypeSymbol.DelegateInvokeMethod?.Parameters.Length == 2)
            {
                return ObservableFromEventWithConvertString;
            }

            return ObservableFromEventString;
        }

        private static string ArgType(IEventSymbol eventSymbol)
        {
            if (eventSymbol.Type is INamedTypeSymbol namedTypeSymbol &&
                namedTypeSymbol.DelegateInvokeMethod?.Parameters != null &&
                namedTypeSymbol.DelegateInvokeMethod.Parameters.Length <= 2)
            {
                if (namedTypeSymbol.DelegateInvokeMethod.Parameters.TryGetLast(out IParameterSymbol parameter))
                {
                    return parameter.Type.ToDisplayString();
                }
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

        private static string Lambda(MethodDeclarationSyntax method, bool usesArg)
        {
            if (usesArg)
            {
                return method.Identifier.ValueText;
            }

            return $"_ => {method.Identifier.ValueText}()";
        }
    }
}