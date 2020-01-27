// ReSharper disable RedundantNameQualifier
namespace Gu.Reactive.Benchmarks
{
    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    public class Cs8602SuppressorBenchmarks
    {
        private static readonly Gu.Roslyn.Asserts.Benchmark Benchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.Cs8602Suppressor());

        [BenchmarkDotNet.Attributes.Benchmark]
        public void RunOnAnalyzerProject()
        {
            Benchmark.Run();
        }
    }
}
