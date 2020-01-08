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

    public static class Program
    {
        public static void Main()
        {
            if (false)
            {
                var benchmark = Gu.Roslyn.Asserts.Benchmark.Create(Code.AnalyzersProject, new InvocationAnalyzer());

                // Warmup
                benchmark.Run();
                Console.WriteLine("Attach profiler and press any key to continue...");
                _ = Console.ReadKey();
                benchmark.Run();
            }
            else
            {
                foreach (var summary in RunSingle<Diff>())
                {
                    CopyResult(summary);
                }
            }
        }

        private static IEnumerable<Summary> RunAll() => new BenchmarkSwitcher(typeof(Program).Assembly).RunAll();

        private static IEnumerable<Summary> RunSingle<T>() => new[] { BenchmarkRunner.Run<T>() };

        private static void CopyResult(Summary summary)
        {
            var name = summary.Title.Split('.').LastOrDefault()?.Split('-').FirstOrDefault();
            if (name is null)
            {
                Console.WriteLine("Did not find name in: " + summary.Title);
                Console.WriteLine("Press any key to exit.");
                _ = Console.ReadKey();
                return;
            }

            var pattern = $"*{name}-report-github.md";
            var sourceFileName = Directory.EnumerateFiles(summary.ResultsDirectoryPath, pattern)
                                          .SingleOrDefault();
            if (sourceFileName is null)
            {
                Console.WriteLine("Did not find a file matching the pattern: " + pattern);
                Console.WriteLine("Press any key to exit.");
                _ = Console.ReadKey();
                return;
            }

            var destinationFileName = Path.ChangeExtension(FindCsFile(), ".md");
            Console.WriteLine($"Copy:");
            Console.WriteLine($"Source: {sourceFileName}");
            Console.WriteLine($"Target: {destinationFileName}");
            File.Copy(sourceFileName, destinationFileName, overwrite: true);

            string FindCsFile()
            {
                return Directory.EnumerateFiles(
                                    AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\" }, StringSplitOptions.RemoveEmptyEntries).First(),
                                    $"{name}.cs",
                                    SearchOption.AllDirectories)
                                .Single();
            }
        }
    }
}
