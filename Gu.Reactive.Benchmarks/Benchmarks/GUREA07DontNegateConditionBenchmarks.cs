// ReSharper disable RedundantNameQualifier
namespace Gu.Reactive.Benchmarks
{
    public class GUREA07DontNegateConditionBenchmarks
    {
        private static readonly Gu.Roslyn.Asserts.Benchmark Benchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA07DontNegateCondition());

        [BenchmarkDotNet.Attributes.Benchmark]
        public void RunOnIDisposableAnalyzers()
        {
            Benchmark.Run();
        }
    }
}
