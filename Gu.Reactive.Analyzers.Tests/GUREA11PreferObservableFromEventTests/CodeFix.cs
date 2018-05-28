namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Reactive.Analyzers.CodeFixes;
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;

    public partial class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new GUREA11PreferObservableFromEvent();
        private static readonly CodeFixProvider Fix = new EventSubscriptionToObserveFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("GUREA11");
    }
}
