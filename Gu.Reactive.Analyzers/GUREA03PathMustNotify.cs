namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class GUREA03PathMustNotify : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "GUREA03";

        private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Path must notify.",
            messageFormat: "Path must notify.",
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
            var method = context.SemanticModel.GetSymbolSafe(invocation, context.CancellationToken);
            if (method == KnownSymbol.NotifyPropertyChangedExt.ObservePropertyChanged ||
                method == KnownSymbol.NotifyPropertyChangedExt.ObservePropertyChangedSlim ||
                method == KnownSymbol.NotifyPropertyChangedExt.ObserveFullPropertyPathSlim)
            {
                if (invocation.ArgumentList != null &&
                    invocation.ArgumentList.Arguments.TryGetFirst(out ArgumentSyntax argument))
                {
                    if (argument.Expression is SimpleLambdaExpressionSyntax lambda)
                    {
                        var memberAccess = lambda.Body as MemberAccessExpressionSyntax;
                        if (memberAccess == null)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Descriptor, lambda.GetLocation()));
                            return;
                        }

                        while (memberAccess != null)
                        {
                            var symbol = context.SemanticModel.GetSymbolSafe(memberAccess, context.CancellationToken);
                            if (!(symbol is IPropertySymbol))
                            {
                                context.ReportDiagnostic(Diagnostic.Create(Descriptor, memberAccess.Name.GetLocation()));
                            }

                            if (symbol.ContainingType.IsValueType ||
                                !symbol.ContainingType.Is(KnownSymbol.INotifyPropertyChanged))
                            {
                                context.ReportDiagnostic(Diagnostic.Create(Descriptor, memberAccess.Name.GetLocation()));
                            }

                            if (symbol.DeclaringSyntaxReferences.Length > 0)
                            {
                                foreach (var reference in symbol.DeclaringSyntaxReferences)
                                {
                                    var propertyDeclaration = (PropertyDeclarationSyntax)reference.GetSyntax(context.CancellationToken);
                                    if (propertyDeclaration.TryGetSetAccessorDeclaration(out AccessorDeclarationSyntax setter) &&
                                        setter.Body == null)
                                    {
                                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, memberAccess.Name.GetLocation()));
                                    }
                                }
                            }

                            memberAccess = memberAccess.Expression as MemberAccessExpressionSyntax;
                        }
                    }
                }
            }
        }
    }
}