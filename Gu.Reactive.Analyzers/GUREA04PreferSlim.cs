namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GUREA04PreferSlim : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GUREA04";

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Prefer slim overload.",
            messageFormat: "Prefer slim overload.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: "The slim overloads do not allocate.",
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
            var method = (IMethodSymbol)context.SemanticModel.GetSymbolSafe(invocation, context.CancellationToken);
            if (method != KnownSymbol.NotifyPropertyChangedExt.ObservePropertyChanged)
            {
                return;
            }

            var argument = invocation.FirstAncestor<ArgumentSyntax>();
            if (argument != null)
            {
                if (argument.TryGetParameter(context.SemanticModel, context.CancellationToken, out IParameterSymbol parameter) &&
                    parameter.Type == KnownSymbol.IObservableOfT)
                {
                    if (parameter.Type is INamedTypeSymbol namedType &&
                        namedType.TypeArguments[0] == KnownSymbol.Object)
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
    }
}