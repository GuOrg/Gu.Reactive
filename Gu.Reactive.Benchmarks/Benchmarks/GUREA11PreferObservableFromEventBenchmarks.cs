// ReSharper disable RedundantNameQualifier
namespace Gu.Reactive.Benchmarks
{
    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    public class GUREA11PreferObservableFromEventBenchmarks
    {
        private static readonly Gu.Roslyn.Asserts.Benchmark Benchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA11PreferObservableFromEvent());

        [BenchmarkDotNet.Attributes.Benchmark]
        public static void RunOnAnalyzerProject()
        {
            Benchmark.Run();
        }
    }
}
