namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using System.Reflection.Metadata.Ecma335;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GUREA12ObservableFromEventDelegateType : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GUREA12";

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Use correct delegate type.",
            messageFormat: "Use correct delegate type.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
            ImmutableArray.Create(Descriptor);

        /// <inheritdoc/>
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(HandleInvocation, SyntaxKind.InvocationExpression);
        }

        private static void HandleInvocation(SyntaxNodeAnalysisContext context)
        {
            if (context.IsExcludedFromAnalysis())
            {
                return;
            }

            var invocation = (InvocationExpressionSyntax)context.Node;
            var method = (IMethodSymbol)context.SemanticModel.GetSymbolSafe(invocation, context.CancellationToken);
            if (method == KnownSymbol.Observable.FromEvent &&
                method.Parameters.Length == 2 &&
                IsForEventHandler(method.Parameters[0]))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocation.GetLocation()));
            }
        }

        private static bool IsForEventHandler(IParameterSymbol parameter)
        {
            if (parameter.Type is INamedTypeSymbol namedType &&
                namedType.Name == "Action" &&
                namedType.TypeArguments.Length == 1 &&
                namedType.TypeArguments[0] is INamedTypeSymbol argType)
            {
                return argType.Name == "EventHandler";
            }

            return false;
        }
    }
}