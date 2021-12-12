namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Roslyn.Asserts;

    public static partial class CodeFix
    {
        private static readonly AddAssignmentAnalyzer Analyzer = new();
        private static readonly EventSubscriptionToObserveFix Fix = new();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA11PreferObservableFromEvent);
    }
}
