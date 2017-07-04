namespace Gu.Reactive.Benchmarks
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using BenchmarkDotNet.Attributes;
    using Gu.Reactive.Internals;

    public class Caching
    {
        private static readonly IReadOnlyList<string> Strings = Enumerable.Range(0, 1000).Select(x => x.ToString()).ToArray();
        private static readonly Expression<Func<Fake, int>> SingleItemPath = x => x.Value;
        private static readonly Expression<Func<Fake, int>> TwoItemPath = x => x.Next.Value;
        private static readonly ConcurrentBag<IdentitySet<string>> Bag = new ConcurrentBag<IdentitySet<string>>(new[] { new IdentitySet<string>(), });
        private static readonly PropertyInfo Property = typeof(Fake).GetProperty("Next");

        [Benchmark(Baseline = true)]
        public int StringGetHashCode()
        {
            return "x => x.Value".GetHashCode();
        }

        [Benchmark]
        public object NewSetPoolIdentitySet()
        {
            return new IdentitySet<string>();
        }

        [Benchmark]
        public object NewSetPoolIdentitySetUnionWithStrings()
        {
            var set = new IdentitySet<string>();
            set.UnionWith(Strings);
            set.Clear();
            return set;
        }

        [Benchmark]
        public object SetPoolBorrowReturn()
        {
            var set = SetPool.Borrow<string>();
            SetPool.Return(set);
            return set;
        }

        [Benchmark]
        public object TryTakeAdd()
        {
            if (Bag.TryTake(out IdentitySet<string> set))
            {
                Bag.Add(set);
            }

            return set;
        }

        [Benchmark]
        public object NewSetPoolBorrowReturnUnionWithStrings()
        {
            var set = SetPool.Borrow<string>();
            set.UnionWith(Strings);
            SetPool.Return(set);
            return set;
        }

        [Benchmark]
        public Expression<Func<Fake, int>> OneLevelExpression()
        {
            return x => x.Value;
        }

        [Benchmark]
        public Expression<Func<Fake, int>> TwoLevelExpression()
        {
            return x => x.Next.Value;
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
