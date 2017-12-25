namespace Gu.Reactive.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GUREA06DontNewCondition
    {
        public const string DiagnosticId = "GUREA06";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Prefer injecting conditions.",
            messageFormat: "Prefer injecting conditions.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false);
    }
}