// ReSharper disable RedundantNameQualifier
namespace Gu.Reactive.Benchmarks
{
    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    public class ConstructorAnalyzerBenchmarks
    {
        private static readonly Gu.Roslyn.Asserts.Benchmark Benchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.ConstructorAnalyzer());

        [BenchmarkDotNet.Attributes.Benchmark]
        public void RunOnAnalyzerProject()
        {
            Benchmark.Run();
        }
    }
}
