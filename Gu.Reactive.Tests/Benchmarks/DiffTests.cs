namespace Gu.Reactive.Tests.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    [Explicit("Longrunning benchmark")]
    public class DiffTests
    {
        [TestCase(1000000)]
        public void Benchmark(int n)
        {
            var fakes = new List<Fake>();
            for (int i = 0; i < n; i++)
            {
                fakes.Add(new Fake { Value = n });
            }

            var warmup = Diff.CollectionChange(fakes, fakes);
            var sw = Stopwatch.StartNew();
            var change = Diff.CollectionChange(fakes, fakes);
            sw.Stop();
            Console.WriteLine("Diff of {0} items took {1} ms", n, sw.ElapsedMilliseconds);
        }
    }
}