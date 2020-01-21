namespace Gu.Reactive.Analyzers.Analyzers
{
    using System.Collections.Immutable;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class Cs8602Suppressor : DiagnosticSuppressor
    {
        private static readonly SuppressionDescriptor Descriptor = new SuppressionDescriptor(
            nameof(Cs8602Suppressor),
            "CS8602",
            "This is always wrong in expressions specifying properties.");

        public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = ImmutableArray.Create(
            Descriptor);

        public override void ReportSuppressions(SuppressionAnalysisContext context)
        {
            foreach (var diagnostic in context.ReportedDiagnostics)
            {
                var root = diagnostic.Location.SourceTree.GetRoot(context.CancellationToken);
                if (root.FindNode(diagnostic.Location.SourceSpan) is { } node &&
                    node.TryFirstAncestorOrSelf(out AnonymousFunctionExpressionSyntax? lambda) &&
                    lambda.TryFirstAncestor(out ArgumentSyntax? argument) &&
                    argument is { Parent: ArgumentListSyntax { Parent: InvocationExpressionSyntax invocation } } &&
                    IsGuReactive(invocation.MethodName()))
                {
                    context.ReportSuppression(Suppression.Create(Descriptor, diagnostic));
                }

                bool IsGuReactive(string? name)
                {
                    return name switch
                    {
                        "ObservePropertyChanged" => true,
                        "ObservePropertyChangedSlim" => true,
                        "ObserveFullPropertyPathSlim" => true,
                        "ObservePropertyChangedWithValue" => true,
                        "ObserveValue" => true,
                        "ObserveItemPropertyChanged" => true,
                        "ObserveItemPropertyChangedSlim" => true,
                        "ItemPropertyChanged" => true,
                        "ItemPropertyChangedSlim" => true,
                        "Property" => true,
                        "TrackMinMax" => true,
                        _ => false,
                    };
                }
            }
        }
    }
}
