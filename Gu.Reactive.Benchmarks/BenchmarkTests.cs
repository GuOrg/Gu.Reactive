namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Gu.Reactive.Analyzers;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class BenchmarkTests
    {
        private static IReadOnlyList<DiagnosticAnalyzer> AllAnalyzers { get; } = typeof(GUREA01DontObserveMutableProperty).Assembly
                                                                                                                          .GetTypes()
                                                                                                                          .Where(typeof(DiagnosticAnalyzer).IsAssignableFrom)
                                                                                                                          .Select(t => (DiagnosticAnalyzer)Activator.CreateInstance(t))
                                                                                                                          .ToArray();

        private static IReadOnlyList<Gu.Roslyn.Asserts.Benchmark> AllBenchmarks { get; } = AllAnalyzers
            .Select(x => Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, x))
            .ToArray();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            foreach (var walker in AllBenchmarks)
            {
                walker.Run();
            }
        }

        [TestCaseSource(nameof(AllBenchmarks))]
        public void Run(Gu.Roslyn.Asserts.Benchmark walker)
        {
            walker.Run();
        }

        [Test]
        public void BenchmarksDirectoryExists()
        {
            Assert.AreEqual(true, Directory.Exists(CodeGen.BenchmarksDirectory), CodeGen.BenchmarksDirectory);
        }
    }
}
