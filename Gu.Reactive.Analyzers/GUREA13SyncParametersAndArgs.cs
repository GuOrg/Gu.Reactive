namespace Gu.Reactive.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GUREA13SyncParametersAndArgs
    {
        public const string DiagnosticId = "GUREA13";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Parameters and arguments for base initializer should have the same order.",
            messageFormat: "Parameters and arguments for base initializer should have the same order.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false);
    }
}