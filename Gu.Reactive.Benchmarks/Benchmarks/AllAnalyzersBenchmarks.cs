// ReSharper disable RedundantNameQualifier
namespace Gu.Reactive.Benchmarks
{
    public class AllAnalyzersBenchmarks
    {
        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA01 = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA01DontObserveMutableProperty());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA02 = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA02ObservableAndCriteriaMustMatch());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA03 = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA03PathMustNotify());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA04 = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA04PreferSlim());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA05 = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA05FullPathMustHaveMoreThanOneItem());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA06 = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA06DontNewCondition());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA07 = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA07DontNegateCondition());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA08 = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA08InlineSingleLine());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA09 = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA09ObservableBeforeCriteria());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA10 = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA10DontMergeInObservable());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA11 = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA11PreferObservableFromEvent());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA12 = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA12ObservableFromEventDelegateType());

        private static readonly Gu.Roslyn.Asserts.Benchmark GUREA13 = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new Gu.Reactive.Analyzers.GUREA13SyncParametersAndArgs());

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA01DontObserveMutableProperty()
        {
            GUREA01.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA02ObservableAndCriteriaMustMatch()
        {
            GUREA02.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA03PathMustNotify()
        {
            GUREA03.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA04PreferSlim()
        {
            GUREA04.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA05FullPathMustHaveMoreThanOneItem()
        {
            GUREA05.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA06DontNewCondition()
        {
            GUREA06.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA07DontNegateCondition()
        {
            GUREA07.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA08InlineSingleLine()
        {
            GUREA08.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA09ObservableBeforeCriteria()
        {
            GUREA09.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA10DontMergeInObservable()
        {
            GUREA10.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA11PreferObservableFromEvent()
        {
            GUREA11.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA12ObservableFromEventDelegateType()
        {
            GUREA12.Run();
        }

        [BenchmarkDotNet.Attributes.Benchmark]
        public void GUREA13SyncParametersAndArgs()
        {
            GUREA13.Run();
        }
    }
}
