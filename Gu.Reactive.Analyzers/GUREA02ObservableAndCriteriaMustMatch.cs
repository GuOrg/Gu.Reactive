namespace Gu.Reactive.Analyzers
{
    using Microsoft.CodeAnalysis;

    public static class GUREA02ObservableAndCriteriaMustMatch
    {
        public const string DiagnosticId = "GUREA02";

        internal static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: DiagnosticId,
            title: "Observable and criteria must match.",
            messageFormat: "Observable and criteria must match.\r\n" +
                           "Observed:\r\n" +
                           "{0}\r\n" +
                           "Used in criteria:\r\n" +
                           "{1}\r\n" +
                           "Not observed:\r\n" +
                           "{2}",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
    }
}