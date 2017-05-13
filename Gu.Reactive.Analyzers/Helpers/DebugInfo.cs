namespace Gu.Reactive.Analyzers
{
    using System;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal static class DebugInfo
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
            id: "GUERROR",
            title: "ERROR",
            messageFormat: MessageFormat,
            category: "ERROR",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: "An error occured.",
            helpLinkUri: @"https://github.com/JohanLarsson/Gu.Reactive");

        private const string MessageFormat = "{0} {1}";

        internal static void RegisterSyntaxNodeActionDebug<TLanguageKindEnum>(this AnalysisContext context, Action<SyntaxNodeAnalysisContext> action, params TLanguageKindEnum[] syntaxKinds)
            where TLanguageKindEnum : struct
        {
            context.RegisterSyntaxNodeAction(
                c =>
                    {
                        try
                        {
                            action(c);
                        }
#pragma warning disable 168
                        catch (Exception e)
#pragma warning restore 168
                        {
                            c.ReportDiagnostic(
                                Diagnostic.Create(
                                    Descriptor,
                                    c.Node.GetLocation(),
                                    c.SemanticModel.SyntaxTree.FilePath,
                                    c.Node));
                            ////throw;
                        }
                    },
                syntaxKinds);
        }
    }
}