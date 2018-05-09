// ReSharper disable HeuristicUnreachableCode
#pragma warning disable 162
namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using BenchmarkDotNet.Reports;
    using BenchmarkDotNet.Running;
    using Gu.Reactive.Analyzers;
    using Gu.Reactive.Internals;
    using Gu.Roslyn.Asserts;

    public class Program
    {
        public static string ProjectDirectory => ProjectFile.Find("Gu.Reactive.Benchmarks.csproj").DirectoryName;

        public static string BenchmarksDirectory { get; } = Path.Combine(ProjectDirectory, "Benchmarks");

        public static void Main()
        {
            if (false)
            {
                var benchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new InvocationAnalyzer());

                // Warmup
                benchmark.Run();
                Console.WriteLine("Attach profiler and press any key to continue...");
                Console.ReadKey().IgnoreReturnValue();
                benchmark.Run();
            }
            else if (true)
            {
                foreach (var summary in RunSingle<InvocationAnalyzerBenchmarks>())
                {
                    CopyResult(summary);
                }
            }
            else
            {
                foreach (var summary in RunAll())
                {
                    CopyResult(summary);
                }
            }
        }

        private static IEnumerable<Summary> RunAll()
        {
            var switcher = new BenchmarkSwitcher(typeof(Program).Assembly);
            var summaries = switcher.Run(new[] { "*" });
            return summaries;
        }

        private static IEnumerable<Summary> RunSingle<T>()
        {
            yield return BenchmarkRunner.Run<T>();
        }

        private static void CopyResult(Summary summary)
        {
            var sourceFileName = Directory.EnumerateFiles(summary.ResultsDirectoryPath, $"*{summary.Title}-report-github.md")
                                          .Single();
            var destinationFileName = Path.Combine(summary.ResultsDirectoryPath, "..\\..\\Benchmarks", summary.Title + ".md");
            Console.WriteLine($"Copy: {sourceFileName} -> {destinationFileName}");
            File.Copy(sourceFileName, destinationFileName, overwrite: true);
        }
    }
}
