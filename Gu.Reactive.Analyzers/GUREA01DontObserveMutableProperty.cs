namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GUREA01DontObserveMutableProperty : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GUREA01";
        private const string Title = "Don't observe mutable property.";
        private const string MessageFormat = "Don't observe mutable property.";
        private const string Description = "Don't observe mutable property.";
        private static readonly string HelpLink = @"https://www.google.com";

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: Title,
            messageFormat: MessageFormat,
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Description,
            helpLinkUri: HelpLink);

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
            if (method == KnownSymbol.NotifyPropertyChangedExt.ObservePropertyChanged ||
                method == KnownSymbol.NotifyPropertyChangedExt.ObservePropertyChangedSlim ||
                method == KnownSymbol.NotifyPropertyChangedExt.ObserveFullPropertyPathSlim)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    if (memberAccess.Expression is MemberAccessExpressionSyntax member)
                    {
                        var symbol = context.SemanticModel.GetSymbolSafe(member, context.CancellationToken);
                        if (IsMutable(symbol))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, memberAccess.Name.GetLocation()));
                        }
                    }
                }
            }
        }

        private static bool IsMutable(ISymbol symbol)
        {
            if (symbol is IPropertySymbol property &&
                !property.IsReadOnly)
            {
                return true;
            }

            if (symbol is IFieldSymbol field &&
                !field.IsReadOnly)
            {
                return true;
            }

            return false;
        }
    }
}
