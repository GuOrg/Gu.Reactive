﻿namespace Gu.Reactive.Analyzers.Tests.GUREA13SyncParametersAndArgsTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly ConstructorAnalyzer Analyzer = new();
        private static readonly SortArgsFix SortArgsFix = new();
        private static readonly SortParametersFix SortParametersFix = new();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA13SyncParametersAndArgs);

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

        private const string Condition3 = @"
namespace N
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class Condition3 : Condition
    {
        public Condition3()
            : base(Observable.Never<object>(), () => true)
        {
        }
    }
}";

        [Test]
        public static void AndConditionSortArgs()
        {
            var before = @"
namespace N
{
    using Gu.Reactive;

    public class FooCondition : AndCondition
    {
        public FooCondition(Condition1 condition1, Condition2 condition2)
            : base↓(condition2, condition1)
        {
        }
    }
}";

            var after = @"
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
            RoslynAssert.CodeFix(Analyzer, SortArgsFix, ExpectedDiagnostic, new[] { Condition1, Condition2, before }, after);
        }

        [Test]
        public static void OrConditionSortArgs()
        {
            var before = @"
namespace N
{
    using Gu.Reactive;

    public class FooCondition : OrCondition
    {
        public FooCondition(
            Condition1 condition1,
            Condition2 condition2,
            Condition3 condition3)
            : base↓(
                condition2,
                condition3,
                condition1)
        {
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Reactive;

    public class FooCondition : OrCondition
    {
        public FooCondition(
            Condition1 condition1,
            Condition2 condition2,
            Condition3 condition3)
            : base(
                condition1,
                condition2,
                condition3)
        {
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, SortArgsFix, ExpectedDiagnostic, new[] { Condition1, Condition2, Condition3, before }, after);
        }

        [Test]
        public static void AndConditionSortParameters()
        {
            var before = @"
namespace N
{
    using Gu.Reactive;

    public class FooCondition : AndCondition
    {
        public FooCondition(Condition1 condition1, Condition2 condition2)
            : base↓(condition2, condition1)
        {
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Reactive;

    public class FooCondition : AndCondition
    {
        public FooCondition(Condition2 condition2, Condition1 condition1)
            : base(condition2, condition1)
        {
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, SortParametersFix, ExpectedDiagnostic, new[] { Condition1, Condition2, before }, after);
        }

        [Test]
        public static void OrConditionSortParameters()
        {
            var before = @"
namespace N
{
    using Gu.Reactive;

    public class FooCondition : OrCondition
    {
        public FooCondition(
            Condition2 condition2,
            Condition3 condition3,
            Condition1 condition1)
            : base↓(
                condition1,
                condition2,
                condition3)
        {
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Reactive;

    public class FooCondition : OrCondition
    {
        public FooCondition(
            Condition1 condition1,
            Condition2 condition2,
            Condition3 condition3)
            : base(
                condition1,
                condition2,
                condition3)
        {
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, SortParametersFix, ExpectedDiagnostic, new[] { Condition1, Condition2, Condition3, before }, after);
        }
    }
}
