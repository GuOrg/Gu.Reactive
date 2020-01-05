namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Simplification;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EventSubscriptionToObserveFix))]
    [Shared]
    public class EventSubscriptionToObserveFix : DocumentEditorCodeFixProvider
    {
        private const string ObservableFromEventString = @"Observable.FromEvent<HANDLERTYPE, ARGTYPE>(
                h => LEFT += h,
                h => LEFT -= h)
                      .Subscribe(LAMBDA)";

        private const string ObservableFromEventWithConvertString = @"Observable.FromEvent<HANDLERTYPE, ARGTYPE>(
                h => (_, e) => h(e),
                h => LEFT += h,
                h => LEFT -= h)
                      .Subscribe(LAMBDA)";

        private static readonly UsingDirectiveSyntax SystemReactiveLinq = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Reactive.Linq"));

        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(Descriptors.GUREA11PreferObservableFromEvent.Id);

        protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                                            .ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNode(diagnostic, out AssignmentExpressionSyntax? assignment) &&
                    semanticModel.GetSymbolSafe(assignment.Left, context.CancellationToken) is IEventSymbol eventSymbol)
                {
                    switch (assignment.Right)
                    {
                        case ParenthesizedLambdaExpressionSyntax { Body: { }, ParameterList: { Parameters: { Count: 2 } } } lambda:
                            {
                                using var pooled = IdentifierNameWalker.Borrow(lambda.Body);
                                var usesArg = false;
                                foreach (var name in pooled.IdentifierNames)
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
                                    "Observe.Event",
                                    editor => editor.AddUsing(SystemReactiveLinq)
                                                    .ReplaceNode(
                                                        assignment,
                                                        Replacement(editor)),
                                    nameof(EventSubscriptionToObserveFix),
                                    diagnostic);
                                break;

                                ExpressionSyntax Replacement(DocumentEditor editor)
                                {
                                    var observeSubscribe = GetObservableFromEventString(eventSymbol)
                                                           .Replace("HANDLERTYPE", eventSymbol.Type.ToMinimalDisplayString(editor.SemanticModel, assignment!.SpanStart))
                                                           .Replace("ARGTYPE", ArgType(eventSymbol))
                                                           .Replace("LEFT", assignment.Left.ToString())
                                                           .Replace("LAMBDA", Lambda((ParenthesizedLambdaExpressionSyntax)assignment.Right, usesArg));

                                    return SyntaxFactory.ParseExpression(observeSubscribe)
                                                        .WithTriviaFrom(assignment)
                                                        .WithSimplifiedNames()
                                                        .WithAdditionalAnnotations(
                                                            Formatter.Annotation, Simplifier.Annotation);
                                }
                            }

                        case MemberAccessExpressionSyntax memberAccess:
                            {
                                if (semanticModel.GetSymbolSafe(memberAccess, context.CancellationToken) is IMethodSymbol { DeclaredAccessibility: Accessibility.Private } method &&
                                    method.TrySingleMethodDeclaration(context.CancellationToken, out var methodDeclaration))
                                {
                                    context.RegisterCodeFix(
                                        "Observe.Event",
                                        editor => Fix(editor),
                                        nameof(EventSubscriptionToObserveFix),
                                        diagnostic);

                                    void Fix(DocumentEditor editor)
                                    {
                                        var usesArg = false;
                                        if (methodDeclaration!.ParameterList.Parameters.Any())
                                        {
                                            using var pooled = IdentifierNameWalker.Borrow((SyntaxNode)methodDeclaration.Body ?? methodDeclaration.ExpressionBody);
                                            foreach (var name in pooled.IdentifierNames)
                                            {
                                                if (name.Identifier.ValueText == methodDeclaration.ParameterList.Parameters[0].Identifier.ValueText)
                                                {
                                                    if (methodDeclaration.ParameterList.Parameters.Count == 1)
                                                    {
                                                        usesArg = true;
                                                        continue;
                                                    }

                                                    return;
                                                }

                                                if (methodDeclaration.ParameterList.Parameters.Count == 2 &&
                                                    name.Identifier.ValueText == methodDeclaration.ParameterList.Parameters[1].Identifier.ValueText)
                                                {
                                                    usesArg = true;
                                                }
                                            }
                                        }

                                        var observeSubscribe = GetObservableFromEventString(eventSymbol)
                                            .Replace("HANDLERTYPE", eventSymbol.Type.ToMinimalDisplayString(editor.SemanticModel, assignment!.SpanStart))
                                            .Replace("ARGTYPE", ArgType(eventSymbol))
                                            .Replace("LEFT", assignment.Left.ToString())
                                            .Replace("LAMBDA", Lambda(methodDeclaration, usesArg));
                                        editor.AddUsing(SystemReactiveLinq)
                                              .ReplaceNode(
                                                  assignment,
                                                  SyntaxFactory.ParseExpression(observeSubscribe)
                                                               .WithTriviaFrom(assignment)
                                                               .WithSimplifiedNames()
                                                               .WithAdditionalAnnotations(Formatter.Annotation, Simplifier.Annotation));
                                        if (methodDeclaration.ParameterList.Parameters.Count == 2)
                                        {
                                            editor.RemoveNode(methodDeclaration.ParameterList.Parameters[0]);
                                        }

                                        if (!usesArg &&
                                            methodDeclaration.ParameterList.Parameters.Any())
                                        {
                                            editor.RemoveNode(methodDeclaration.ParameterList.Parameters.Last());
                                        }
                                    }
                                }

                                break;
                            }
                    }
                }
            }
        }

        private static string GetObservableFromEventString(IEventSymbol eventSymbol)
        {
            if (eventSymbol.Type is INamedTypeSymbol { DelegateInvokeMethod: { Parameters: { Length: 2 } } })
            {
                return ObservableFromEventWithConvertString;
            }

            return ObservableFromEventString;
        }

        private static string ArgType(IEventSymbol eventSymbol)
        {
            if (eventSymbol.Type is INamedTypeSymbol { DelegateInvokeMethod: { Parameters: { } parameters } } &&
                parameters.Length <= 2 &&
                parameters.TryLast(out IParameterSymbol? parameter))
            {
                return parameter!.Type.ToDisplayString();
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
