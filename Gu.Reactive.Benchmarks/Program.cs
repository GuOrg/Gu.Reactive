namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using BenchmarkDotNet.Reports;
    using BenchmarkDotNet.Running;
    using Gu.Reactive.Analyzers;
    using Gu.Roslyn.Asserts;

    public class Program
    {
        public static string ProjectDirectory => CodeFactory.FindProjectFile("Gu.Reactive.Benchmarks.csproj").DirectoryName;

        public static string BenchmarksDirectory { get; } = Path.Combine(ProjectDirectory, "Benchmarks");

        public static void Main()
        {
            if (false)
            {
                var benchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new GUREA01DontObserveMutableProperty());

                // Warmup
                benchmark.Run();
                Console.WriteLine("Attach profiler and press any key to continue...");
                Console.ReadKey();
                benchmark.Run();
            }
            else if (true)
            {
                foreach (var summary in RunSingle<GUREA13Benchmarks>())
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
            var summaries = new[] { BenchmarkRunner.Run<T>() };
            return summaries;
        }

        private static void CopyResult(Summary summary)
        {
            Console.WriteLine($"DestinationDirectory: {BenchmarksDirectory}");
            if (Directory.Exists(BenchmarksDirectory))
            {
                var sourceFileName = Directory.EnumerateFiles(summary.ResultsDirectoryPath)
                                              .Single(x => x.EndsWith(summary.Title + "-report-github.md"));
                var destinationFileName = Path.Combine(BenchmarksDirectory, summary.Title + ".md");
                Console.WriteLine($"Copy: {sourceFileName} -> {destinationFileName}");
                File.Copy(sourceFileName, destinationFileName, overwrite: true);
            }
        }
    }
}
