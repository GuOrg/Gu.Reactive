namespace Gu.Reactive.Analyzers.Tests.GUREA11ObserveTests
{
    using Gu.Roslyn.Asserts;

    public partial class CodeFix
    {
        static CodeFix()
        {
            AnalyzerAssert.MetadataReference.AddRange(MetadataReferences.All);
        }
    }
}