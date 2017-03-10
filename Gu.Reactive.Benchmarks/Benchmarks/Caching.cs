namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using BenchmarkDotNet.Attributes;
    using Gu.Reactive.Internals;

    public class Caching
    {
        private static readonly Expression<Func<Fake, int>> SingleItemPath = x => x.Value;
        private static readonly Expression<Func<Fake, int>> TwoItemPath = x => x.Next.Value;
        private static readonly PropertyInfo Property = typeof(Fake).GetProperty("Next");

        [Benchmark(Baseline = true)]
        public int StringGetHashCode()
        {
            return "x => x.Value".GetHashCode();
        }

        [Benchmark]
        public int PropertyPathComparerGetHashCodeSingleItemPath()
        {
            return ((IEqualityComparer<LambdaExpression>)PropertyPathComparer.Default).GetHashCode(SingleItemPath);
        }

        [Benchmark]
        public int PropertyPathComparerGetHashCodeTwoItemPath()
        {
            return ((IEqualityComparer<LambdaExpression>)PropertyPathComparer.Default).GetHashCode(TwoItemPath);
        }

        [Benchmark]
        public object NotifyingPathGetOrCreateSingleItemPath()
        {
            return NotifyingPath.GetOrCreate(SingleItemPath);
        }

        [Benchmark]
        public object NotifyingPathGetOrCreateTwoItemPath()
        {
            return NotifyingPath.GetOrCreate(TwoItemPath);
        }

        [Benchmark]
        public object GetterGetOrCreateProperty()
        {
            return Getter.GetOrCreate(Property);
        }
    }
}
