namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GUREA11Observe : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GUREA11";

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Prefer observing",
            messageFormat: "Prefer observing",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Prefer observing",
            helpLinkUri: @"https://github.com/JohanLarsson/Gu.Reactive");

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(HandleInvocation, SyntaxKind.AddAssignmentExpression);
        }

        private static void HandleInvocation(SyntaxNodeAnalysisContext context)
        {
            if (context.IsExcludedFromAnalysis())
            {
                return;
            }

            var assignment = (AssignmentExpressionSyntax)context.Node;
            var left = context.SemanticModel.GetSymbolSafe(assignment.Left, context.CancellationToken);
            if (left is IEventSymbol)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, assignment.OperatorToken.GetLocation()));
            }
        }
    }
}