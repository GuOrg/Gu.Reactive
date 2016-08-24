namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    using BenchmarkDotNet.Attributes;

    public class Diff
    {
        private static List<Fake> _x;
        private static List<Fake> _y;


        [Params(10, 100, 1000)]
        public int N
        {
            set
            {
                _x = CreateFakes(value);
                _y = CreateFakes(value);
            }
        }

        [Benchmark]
        public NotifyCollectionChangedEventArgs CollectionChange()
        {
            return Reactive.Diff.CollectionChange(_x, _y);
        }

        private static List<Fake> CreateFakes(int n)
        {
            return Enumerable.Range(0, n)
                             .Select(x => new Fake { Value = x })
                             .ToList();
        }
    }
}