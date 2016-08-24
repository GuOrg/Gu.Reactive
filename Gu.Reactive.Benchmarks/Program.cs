namespace Gu.Reactive.Benchmarks
{
    using System.Collections.Generic;
    using System.IO;

    using BenchmarkDotNet.Reports;
    using BenchmarkDotNet.Running;

    public class Program
    {
        public static void Main()
        {
            foreach (var summary in RunSingle<ThrottledView>())
            {
                CopyResult(summary.Title);
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

        private static void CopyResult(string name)
        {
#if DEBUG
#else
            var sourceFileName = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "BenchmarkDotNet.Artifacts", "results", name + "-report-github.md");
            var destinationDirectory = System.IO.Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Results");
            System.IO.Directory.CreateDirectory(destinationDirectory);
            var destinationFileName = System.IO.Path.Combine(destinationDirectory, name + ".md");
            File.Copy(sourceFileName, destinationFileName, true);
#endif
        }
    }
}
