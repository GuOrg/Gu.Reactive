namespace Gu.Reactive.Analyzers
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
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
            messageFormat: "Observable and criteria must match.\r\n" +
                           "Observed:\r\n" +
                           "{0}\r\n" +
                           "Used in criteria:\r\n" +
                           "{1}\r\n" +
                           "Not observed:\r\n" +
                           "{2}",
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
                        using (var observed = SetPool<IPropertySymbol>.Create())
                        {
                            foreach (var name in observableIdentifiers.Item.IdentifierNames)
                            {
                                if (semanticModel.GetSymbolSafe(name, cancellationToken) is IPropertySymbol property)
                                {
                                    observed.Item.Add(property);
                                }
                            }

                            using (var usedInCriteria = SetPool<IPropertySymbol>.Create())
                            {
                                foreach (var name in criteriaIdentifiers.Item.IdentifierNames)
                                {
                                    if (semanticModel.GetSymbolSafe(name, cancellationToken) is IPropertySymbol property)
                                    {
                                        if (!property.IsGetOnly())
                                        {
                                            usedInCriteria.Item.Add(property);
                                        }
                                    }
                                }

                                using (var missing = SetPool<IPropertySymbol>.Create())
                                {
                                    missing.Item.UnionWith(usedInCriteria.Item);
                                    missing.Item.ExceptWith(observed.Item);
                                    if (missing.Item.Count != 0)
                                    {
                                        var observedText = string.Join(Environment.NewLine, observed.Item.Select(p => $"  {p}"));
                                        var usedInCriteriaText = string.Join(Environment.NewLine, usedInCriteria.Item.Select(p => $"  {p}"));
                                        var missingText = string.Join(Environment.NewLine, missing.Item.Select(p => $"  {p}"));
                                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, initializer.GetLocation(), observedText, usedInCriteriaText, missingText));
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