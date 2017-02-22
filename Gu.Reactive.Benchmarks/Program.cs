namespace Gu.Reactive.Benchmarks
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using BenchmarkDotNet.Reports;
    using BenchmarkDotNet.Running;

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    public class Program
    {
        //// ReSharper disable PossibleNullReferenceException
        private static readonly string DesinationDirectory = System.IO.Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Benchmarks");
        //// ReSharper restore PossibleNullReferenceException

        public static void Main()
        {
            foreach (var summary in RunSingle<ObservePropertyChangedThenSubscribeThenReact>())
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
            System.IO.Directory.CreateDirectory(DesinationDirectory);
            var destinationFileName = System.IO.Path.Combine(DesinationDirectory, name + ".md");
            File.Copy(sourceFileName, destinationFileName, true);
#endif
        }
    }
}
