﻿namespace Gu.Reactive.Analyzers.Tests.GUREA13SyncParametersAndArgsTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly ConstructorAnalyzer Analyzer = new();

        private const string Condition1 = @"
namespace N
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class Condition1 : Condition
    {
        public Condition1()
            : base(Observable.Never<object>(), () => true)
        {
        }
    }
}";

        private const string Condition2 = @"
namespace N
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class Condition2 : Condition
    {
        public Condition2()
            : base(Observable.Never<object>(), () => true)
        {
        }
    }
}";

        [Test]
        public static void AndConditionSortArgs()
        {
            var code = @"
namespace N
{
    using Gu.Reactive;

    public class FooCondition : AndCondition
    {
        public FooCondition(Condition1 condition1, Condition2 condition2)
            : base(condition1, condition2)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Condition1, Condition2, code);
        }
    }
}
