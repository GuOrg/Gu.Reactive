// ReSharper disable RedundantNameQualifier
namespace Gu.Reactive.Benchmarks
{
    public class GUREA12ObservableFromEventDelegateTypeBenchmarks
    {
        private static readonly Gu.Roslyn.Asserts.Benchmark Benchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA12ObservableFromEventDelegateType());

        [BenchmarkDotNet.Attributes.Benchmark]
        public void RunOnIDisposableAnalyzers()
        {
            Benchmark.Run();
        }
    }
}
