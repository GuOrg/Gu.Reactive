namespace Gu.Reactive.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GUREA03PathMustNotify
    {
        public const string DiagnosticId = "GUREA03";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Path must notify.",
            messageFormat: "Path must notify.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}