namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GUREA10DontMergeInObservable : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GUREA10";

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Split up into two conditions?",
            messageFormat: "Split up into two conditions?",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(HandleInvocation, SyntaxKind.BaseConstructorInitializer);
        }

        private static void HandleInvocation(SyntaxNodeAnalysisContext context)
        {
            if (context.IsExcludedFromAnalysis())
            {
                return;
            }

            var initializer = (ConstructorInitializerSyntax)context.Node;
            var ctor = context.SemanticModel.GetSymbolSafe(initializer, context.CancellationToken);
            if (ctor == null ||
                ctor.ContainingType != KnownSymbol.Condition ||
                initializer.ArgumentList?.Arguments.Count != 2)
            {
                return;
            }

            var argument = GetObservableArgument(initializer, ctor);
            if (argument != null)
            {
                using (var pooled = InvocationWalker.Create(argument, Search.Recursive, context.SemanticModel, context.CancellationToken))
                {
                    foreach (var invocation in pooled.Item.Invocations)
                    {
                        if (context.SemanticModel.GetSymbolSafe(invocation, context.CancellationToken) == KnownSymbol.Observable.Merge)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, argument.GetLocation()));
                        }
                    }
                }
            }
        }

        private static ArgumentSyntax GetObservableArgument(ConstructorInitializerSyntax initializer, IMethodSymbol ctor)
        {
            if (ctor.Parameters[0].Type == KnownSymbol.IObservableOfT &&
                ctor.Parameters[1].Type == KnownSymbol.FuncOfT)
            {
                return initializer.ArgumentList.Arguments[0];
            }

            if (ctor.Parameters[0].Type == KnownSymbol.FuncOfT &&
                ctor.Parameters[1].Type == KnownSymbol.IObservableOfT)
            {
                return initializer.ArgumentList.Arguments[1];
            }

            return null;
        }
    }
}