// ReSharper disable RedundantNameQualifier
namespace Gu.Reactive.Benchmarks
{
    public class GUREA10DontMergeInObservableBenchmarks
    {
        private static readonly Gu.Roslyn.Asserts.Benchmark Benchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA10DontMergeInObservable());

        [BenchmarkDotNet.Attributes.Benchmark]
        public void RunOnIDisposableAnalyzers()
        {
            Benchmark.Run();
        }
    }
}
