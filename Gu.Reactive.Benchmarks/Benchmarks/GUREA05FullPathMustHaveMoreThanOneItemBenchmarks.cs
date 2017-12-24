// ReSharper disable RedundantNameQualifier
namespace Gu.Reactive.Benchmarks
{
    public class GUREA05FullPathMustHaveMoreThanOneItemBenchmarks
    {
        private static readonly Gu.Roslyn.Asserts.Benchmark Benchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA05FullPathMustHaveMoreThanOneItem());

        [BenchmarkDotNet.Attributes.Benchmark]
        public void RunOnIDisposableAnalyzers()
        {
            Benchmark.Run();
        }
    }
}
