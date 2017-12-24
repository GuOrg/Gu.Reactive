// ReSharper disable RedundantNameQualifier
namespace Gu.Reactive.Benchmarks
{
    public class AllAnalyzersBenchmarks
    {
        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA02ObservableAndCriteriaMustMatchBenchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA02ObservableAndCriteriaMustMatch());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA04PreferSlimBenchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA04PreferSlim());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA06DontNewConditionBenchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA06DontNewCondition());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA08InlineSingleLineBenchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA08InlineSingleLine());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA09ObservableBeforeCriteriaBenchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA09ObservableBeforeCriteria());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA10DontMergeInObservableBenchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA10DontMergeInObservable());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA11PreferObservableFromEventBenchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA11PreferObservableFromEvent());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA13SyncParametersAndArgsBenchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA13SyncParametersAndArgs());

        private static readonly Gu.Roslyn.Asserts.Benchmark InvocationAnalyzerBenchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.InvocationAnalyzer());

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA02ObservableAndCriteriaMustMatch()
        {
            GUREA02ObservableAndCriteriaMustMatchBenchmark.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA04PreferSlim()
        {
            GUREA04PreferSlimBenchmark.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA06DontNewCondition()
        {
            GUREA06DontNewConditionBenchmark.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA08InlineSingleLine()
        {
            GUREA08InlineSingleLineBenchmark.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA09ObservableBeforeCriteria()
        {
            GUREA09ObservableBeforeCriteriaBenchmark.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA10DontMergeInObservable()
        {
            GUREA10DontMergeInObservableBenchmark.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA11PreferObservableFromEvent()
        {
            GUREA11PreferObservableFromEventBenchmark.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA13SyncParametersAndArgs()
        {
            GUREA13SyncParametersAndArgsBenchmark.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void InvocationAnalyzer()
        {
            InvocationAnalyzerBenchmark.Run();
        }
    }
}
