namespace Gu.Reactive.Benchmarks
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using BenchmarkDotNet.Attributes;

    public class Diff
    {
        private static List<Fake> x;
        private static List<Fake> y;

        [Params(10, 100, 1000)]
#pragma warning disable CA1044 // Properties should not be write only
        public int N
#pragma warning restore CA1044 // Properties should not be write only
        {
            set
            {
                x = CreateFakes(value);
                y = CreateFakes(value);
            }
        }

        [Benchmark]
        public NotifyCollectionChangedEventArgs CollectionChange()
        {
            return Reactive.Diff.CollectionChange(x, y);
        }

        private static List<Fake> CreateFakes(int n)
        {
            return Enumerable.Range(0, n)
                             .Select(i => new Fake { Value = i })
                             .ToList();
        }
    }
}
