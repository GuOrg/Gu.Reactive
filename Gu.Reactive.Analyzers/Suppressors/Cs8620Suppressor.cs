namespace Gu.Reactive.Analyzers
{
    using System.Collections.Immutable;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class Cs8620Suppressor : DiagnosticSuppressor
    {
        private static readonly SuppressionDescriptor Descriptor = new SuppressionDescriptor(
            nameof(Cs8620Suppressor),
            "CS8620",
            "No way to declare out T in structs and classes.");

        public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = ImmutableArray.Create(
            Descriptor);

        public override void ReportSuppressions(SuppressionAnalysisContext context)
        {
            foreach (var diagnostic in context.ReportedDiagnostics)
            {
                if (diagnostic.GetMessage(CultureInfo.InvariantCulture) is { } message &&
                    IsOut(message))
                {
                    context.ReportSuppression(Suppression.Create(Descriptor, diagnostic));
                }

                static bool IsOut(string message)
                {
                    return Regex.IsMatch(message, "Argument of type '.*Maybe<.+>>*' cannot be used for parameter '[^']+' of type '.*Maybe<.+\\?>>*'") ||
                           Regex.IsMatch(message, "Argument of type '.*PropertyChangedAndValueEventArgs<.+>>*' cannot be used for parameter '[^']+' of type '.*PropertyChangedAndValueEventArgs<.+\\?>>*'");
                }
            }
        }
    }
}
