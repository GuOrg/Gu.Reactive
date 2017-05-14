namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GUREA08InlineSingleLine : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GUREA08";

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Inline single line.",
            messageFormat: "Inline single line.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            description: "Inline single line.",
            helpLinkUri: @"https://github.com/JohanLarsson/Gu.Reactive");

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
            if (!(invocation.Parent is ArgumentSyntax))
            {
                return;
            }

            var initializer = invocation.FirstAncestor<ConstructorInitializerSyntax>();
            if (initializer == null ||
                context.SemanticModel.GetSymbolSafe(initializer, context.CancellationToken)?.ContainingType != KnownSymbol.Condition)
            {
                return;
            }

            var method = (IMethodSymbol)context.SemanticModel.GetSymbolSafe(invocation, context.CancellationToken);
            if (method.DeclaredAccessibility != Accessibility.Private &&
                !method.IsStatic &&
                method.Parameters.Length == 1)
            {
                return;
            }

            foreach (var reference in method.DeclaringSyntaxReferences)
            {
                var methodDeclaration = (MethodDeclarationSyntax)reference.GetSyntax(context.CancellationToken);
                if (methodDeclaration.ExpressionBody != null ||
                    (methodDeclaration.Body?.Statements.Count == 1 &&
                     methodDeclaration.Body.Statements[0] is ReturnStatementSyntax))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, invocation.GetLocation()));
                }
            }
        }
    }
}