namespace Gu.Reactive.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GUREA05FullPathMustHaveMoreThanOneItem
    {
        public const string DiagnosticId = "GUREA05";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Full path must have more than one item.",
            messageFormat: "Full path must have more than one item.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}