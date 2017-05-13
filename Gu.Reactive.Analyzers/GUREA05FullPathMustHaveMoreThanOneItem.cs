namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GUREA05FullPathMustHaveMoreThanOneItem : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GUREA05";

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Full path must have more than one item.",
            messageFormat: "Full path must have more than one item.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "Full path must have more than one item.",
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
            var method = context.SemanticModel.GetSymbolSafe(invocation, context.CancellationToken);
            if (method == KnownSymbol.NotifyPropertyChangedExt.ObserveFullPropertyPathSlim)
            {
                if (invocation.ArgumentList != null &&
                    invocation.ArgumentList.Arguments.TryGetFirst(out ArgumentSyntax argument))
                {
                    if (argument.Expression is SimpleLambdaExpressionSyntax lambda &&
                        lambda.Body != null)
                    {
                        var memberAccess = lambda.Body as MemberAccessExpressionSyntax;
                        if (memberAccess == null ||
                            !(memberAccess.Expression is MemberAccessExpressionSyntax))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, lambda.Body.GetLocation()));
                            return;
                        }
                    }
                }
            }
        }
    }
}