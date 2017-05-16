namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GUREA02ObservableAndCriteriaMustMatch : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GUREA02";

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Observable and criteria must match.",
            messageFormat: "Observable and criteria must match.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

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
            var semanticModel = context.SemanticModel;
            var cancellationToken = context.CancellationToken;
            var ctor = semanticModel.GetSymbolSafe(initializer, cancellationToken);
            if (ctor != null &&
                ctor.ContainingType == KnownSymbol.Condition &&
                ctor.Parameters.Length == 2 &&
                initializer.ArgumentList != null &&
                initializer.ArgumentList.Arguments.Count == 2)
            {
                var observableArg = ObservableArg(initializer, ctor);
                var criteriaArg = CriteriaArg(initializer, ctor);
                using (var observableIdentifiers = IdentifierNameWalker.Create(observableArg, Search.Recursive, semanticModel, cancellationToken))
                {
                    using (var criteriaIdentifiers = IdentifierNameWalker.Create(criteriaArg, Search.Recursive, semanticModel, cancellationToken))
                    {
                        using (var pooledSet = SetPool<IPropertySymbol>.Create())
                        {
                            foreach (var name in observableIdentifiers.Item.IdentifierNames)
                            {
                                if (semanticModel.GetSymbolSafe(name, cancellationToken) is IPropertySymbol property)
                                {
                                    pooledSet.Item.Add(property);
                                }
                            }

                            foreach (var name in criteriaIdentifiers.Item.IdentifierNames)
                            {
                                if (semanticModel.GetSymbolSafe(name, cancellationToken) is IPropertySymbol property)
                                {
                                    if (pooledSet.Item.Add(property))
                                    {
                                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, initializer.GetLocation()));
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static ArgumentSyntax ObservableArg(ConstructorInitializerSyntax initializer, IMethodSymbol ctor)
        {
            if (ctor.Parameters[0].Type == KnownSymbol.FuncOfT &&
                ctor.Parameters[1].Type == KnownSymbol.IObservableOfT)
            {
                return initializer.ArgumentList.Arguments[1];
            }

            return initializer.ArgumentList.Arguments[0];
        }

        private static ArgumentSyntax CriteriaArg(ConstructorInitializerSyntax initializer, IMethodSymbol ctor)
        {
            if (ctor.Parameters[0].Type == KnownSymbol.FuncOfT &&
                ctor.Parameters[1].Type == KnownSymbol.IObservableOfT)
            {
                return initializer.ArgumentList.Arguments[0];
            }

            return initializer.ArgumentList.Arguments[1];
        }
    }
}