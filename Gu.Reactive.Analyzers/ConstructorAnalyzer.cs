namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConstructorAnalyzer : DiagnosticAnalyzer
    {
        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            GUREA13SyncParametersAndArgs.Descriptor);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(HandleBaseInitializer, SyntaxKind.BaseConstructorInitializer);
        }

        private static void HandleBaseInitializer(SyntaxNodeAnalysisContext context)
        {
            if (context.IsExcludedFromAnalysis())
            {
                return;
            }

            if (context.Node is ConstructorInitializerSyntax initializer &&
                context.SemanticModel.GetSymbolSafe(initializer, context.CancellationToken) is IMethodSymbol baseCtor)
            {
                if (baseCtor.ContainingType.IsEither(KnownSymbol.AndCondition, KnownSymbol.OrCondition) &&
                    HasMatchingArgumentAndParameterPositions(initializer, context) == false)
                {
                    context.ReportDiagnostic(Diagnostic.Create(GUREA13SyncParametersAndArgs.Descriptor, initializer.GetLocation()));
                }
            }
        }

        private static bool? HasMatchingArgumentAndParameterPositions(ConstructorInitializerSyntax initializer, SyntaxNodeAnalysisContext context)
        {
            if (initializer?.ArgumentList == null)
            {
                return null;
            }

            if (context.SemanticModel.GetDeclaredSymbolSafe(initializer.Parent, context.CancellationToken) is IMethodSymbol ctor)
            {
                if (ctor.Parameters.Length != initializer.ArgumentList.Arguments.Count)
                {
                    return null;
                }

                for (var i = 0; i < initializer.ArgumentList.Arguments.Count; i++)
                {
                    var argument = initializer.ArgumentList.Arguments[i];
                    if (argument.Expression is IdentifierNameSyntax argName &&
                        argName.Identifier.ValueText != ctor.Parameters[i].Name)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}