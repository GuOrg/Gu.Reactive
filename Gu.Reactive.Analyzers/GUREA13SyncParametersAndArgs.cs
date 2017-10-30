namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GUREA13SyncParametersAndArgs : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GUREA13";

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Parameters and arguments for base initializer should have the same order.",
            messageFormat: "Parameters and arguments for base initializer should have the same order.",
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
            var baseCtor = context.SemanticModel.GetSymbolSafe(initializer, context.CancellationToken);
            if (baseCtor == null ||
                initializer.ArgumentList == null ||
                !(baseCtor.ContainingType == KnownSymbol.AndCondition ||
                  baseCtor.ContainingType == KnownSymbol.OrCondition))
            {
                return;
            }

            var ctor = (IMethodSymbol)context.ContainingSymbol;
            var isOutOfSync = false;
            for (var i = 0; i < initializer.ArgumentList.Arguments.Count; i++)
            {
                var argument = initializer.ArgumentList.Arguments[i];
                if (context.SemanticModel.GetSymbolSafe(argument.Expression, context.CancellationToken) is IParameterSymbol parameter)
                {
                    if (ctor.Parameters.IndexOf(parameter) != i)
                    {
                        isOutOfSync = true;
                    }
                }
                else
                {
                    return;
                }
            }

            if (isOutOfSync)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, initializer.GetLocation()));
            }
        }
    }
}