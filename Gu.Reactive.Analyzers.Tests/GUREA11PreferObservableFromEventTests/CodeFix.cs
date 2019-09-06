namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    public static partial class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new AddAssignmentAnalyzer();
        private static readonly CodeFixProvider Fix = new EventSubscriptionToObserveFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA11PreferObservableFromEvent);
    }
}
