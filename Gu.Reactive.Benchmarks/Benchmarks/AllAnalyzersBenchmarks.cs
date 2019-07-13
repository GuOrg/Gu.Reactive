// ReSharper disable RedundantNameQualifier
namespace Gu.Reactive.Benchmarks
{
    [BenchmarkDotNet.Attributes.MemoryDiagnoser]
    public class AllAnalyzersBenchmarks
    {
        private static readonly Gu.Roslyn.Asserts.Benchmark ConstructorAnalyzerBenchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.ConstructorAnalyzer());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA11PreferObservableFromEventBenchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA11PreferObservableFromEvent());

        private static readonly Gu.Roslyn.Asserts.Benchmark InvocationAnalyzerBenchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.InvocationAnalyzer());

        [BenchmarkDotNet.Attributes.Benchmark]
        public void ConstructorAnalyzer()
        {
            ConstructorAnalyzerBenchmark.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA11PreferObservableFromEvent()
        {
            GUREA11PreferObservableFromEventBenchmark.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void InvocationAnalyzer()
        {
            InvocationAnalyzerBenchmark.Run();
        }
    }
}
