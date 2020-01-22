namespace Gu.Reactive.Analyzers
{
    using Microsoft.CodeAnalysis;

    internal static class Descriptors
    {
        internal static readonly DiagnosticDescriptor GUREA01DoNotObserveMutableProperty = new DiagnosticDescriptor(
            id: "GUREA01",
            title: "Don't observe mutable property.",
            messageFormat: "Don't observe mutable property.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        internal static readonly DiagnosticDescriptor GUREA02ObservableAndCriteriaMustMatch = new DiagnosticDescriptor(
            id: "GUREA02",
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

        internal static readonly DiagnosticDescriptor GUREA03PathMustNotify = new DiagnosticDescriptor(
            id: "GUREA03",
            title: "Path must notify.",
            messageFormat: "Path must notify.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        internal static readonly DiagnosticDescriptor GUREA04PreferSlimOverload = new DiagnosticDescriptor(
            id: "GUREA04",
            title: "Prefer slim overload.",
            messageFormat: "Prefer slim overload.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        internal static readonly DiagnosticDescriptor GUREA05FullPathMustHaveMoreThanOneItem = new DiagnosticDescriptor(
            id: "GUREA05",
            title: "Full path must have more than one item.",
            messageFormat: "Full path must have more than one item.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        internal static readonly DiagnosticDescriptor GUREA06DoNotNewCondition = new DiagnosticDescriptor(
            id: "GUREA06",
            title: "Prefer injecting conditions.",
            messageFormat: "Prefer injecting conditions.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false);

        internal static readonly DiagnosticDescriptor GUREA07DoNotNegateCondition = new DiagnosticDescriptor(
            id: "GUREA07",
            title: "Prefer injecting conditions.",
            messageFormat: "Prefer injecting conditions.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false);

        internal static readonly DiagnosticDescriptor GUREA08InlineSingleLine = new DiagnosticDescriptor(
            id: "GUREA08",
            title: "Inline single line.",
            messageFormat: "Inline single line.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false);

        internal static readonly DiagnosticDescriptor GUREA09ObservableBeforeCriteria = new DiagnosticDescriptor(
            id: "GUREA09",
            title: "Place observable before criteria.",
            messageFormat: "Place observable before criteria.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false);

        internal static readonly DiagnosticDescriptor GUREA10DoNotMergeInObservable = new DiagnosticDescriptor(
            id: "GUREA10",
            title: "Split up into two conditions?",
            messageFormat: "Split up into two conditions?",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false);

        internal static readonly DiagnosticDescriptor GUREA11PreferObservableFromEvent = new DiagnosticDescriptor(
            id: "GUREA11",
            title: "Prefer observing",
            messageFormat: "Prefer observing",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: true);

        internal static readonly DiagnosticDescriptor GUREA12ObservableFromEventDelegateType = new DiagnosticDescriptor(
            id: "GUREA12",
            title: "Use correct delegate type.",
            messageFormat: "Use correct delegate type.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        internal static readonly DiagnosticDescriptor GUREA13SyncParametersAndArgs = new DiagnosticDescriptor(
            id: "GUREA13",
            title: "Parameters and arguments for base initializer should have the same order.",
            messageFormat: "Parameters and arguments for base initializer should have the same order.",
            category: AnalyzerCategory.Correctness,
            defaultSeverity: DiagnosticSeverity.Hidden,
            isEnabledByDefault: false);
    }
}
