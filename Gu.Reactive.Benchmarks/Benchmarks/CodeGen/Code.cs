namespace Gu.Reactive.Benchmarks
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;

    public static class Code
    {
        public static Solution AnalyzersProject { get; } = CodeFactory.CreateSolution(
            ProjectFile.Find("Gu.Reactive.Benchmarks.csproj"));
    }
}
