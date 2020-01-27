﻿namespace Gu.Reactive.Benchmarks
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using BenchmarkDotNet.Attributes;

    public class Diff
    {
        private List<Fake> x = null!;
        private List<Fake> y = null!;

        [Params(10, 100, 1000)]
#pragma warning disable CA1044 // Properties should not be write only
        public int N
#pragma warning restore CA1044 // Properties should not be write only
        {
            set
            {
                this.x = CreateFakes(value);
                this.y = CreateFakes(value);
            }
        }

        [Benchmark]
        public NotifyCollectionChangedEventArgs? CollectionChange()
        {
            return Reactive.Diff.CollectionChange(this.x, this.y);
        }

        private static List<Fake> CreateFakes(int n)
        {
            return Enumerable.Range(0, n)
                             .Select(i => new Fake { Value = i })
                             .ToList();
        }
    }
}
