namespace Gu.Reactive.Analyzers.Tests
{
    using System.Runtime.CompilerServices;
    using Gu.Roslyn.Asserts;

    internal static class ModuleInitializer
    {
        [ModuleInitializer]
        internal static void Initialize()
        {
            Settings.Default = Settings.Default
                                       .WithCompilationOptions(x => x.WithSuppressedDiagnostics("CS1701"))
                                       .WithMetadataReferences(MetadataReferences.Transitive(typeof(Gu.Wpf.Reactive.AsyncCommand)));
        }
    }
}
