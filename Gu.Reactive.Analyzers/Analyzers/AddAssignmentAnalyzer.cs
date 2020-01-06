namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AddAssignmentAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptors.GUREA11PreferObservableFromEvent);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(HandleInvocation, SyntaxKind.AddAssignmentExpression);
        }

        private static void HandleInvocation(SyntaxNodeAnalysisContext context)
        {
            if (!context.IsExcludedFromAnalysis() &&
                context.Node is AssignmentExpressionSyntax { Left: { } left } assignment &&
                assignment.FirstAncestor<ArgumentSyntax>() is null &&
                context.SemanticModel.GetSymbolSafe(left, context.CancellationToken) is IEventSymbol _)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptors.GUREA11PreferObservableFromEvent, assignment.GetLocation()));
            }
        }
    }
}
