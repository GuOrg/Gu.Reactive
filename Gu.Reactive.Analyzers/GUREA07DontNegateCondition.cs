namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GUREA07DontNegateCondition : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GUREA07";

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Prefer injecting conditions.",
            messageFormat: "Prefer injecting conditions.",
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
            context.RegisterSyntaxNodeAction(HandleCreation, SyntaxKind.InvocationExpression);
        }

        private static void HandleCreation(SyntaxNodeAnalysisContext context)
        {
            if (context.IsExcludedFromAnalysis())
            {
                return;
            }

            var invocation = (InvocationExpressionSyntax)context.Node;
            var method = (IMethodSymbol)context.SemanticModel.GetSymbolSafe(invocation, context.CancellationToken);
            if (method == KnownSymbol.ICondition.Negate)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, memberAccess.Name.GetLocation()));
                }
                else
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocation.GetLocation()));
                }
            }
        }
    }
}