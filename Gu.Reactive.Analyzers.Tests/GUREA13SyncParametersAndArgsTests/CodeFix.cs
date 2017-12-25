namespace Gu.Reactive.Analyzers.Tests.GUREA13SyncParametersAndArgsTests
{
    using Gu.Reactive.Analyzers.CodeFixes;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class CodeFix
    {
        private static readonly ConstructorAnalyzer Analyzer = new ConstructorAnalyzer();
        private static readonly SortArgsCodeFix SortArgsCodeFix = new SortArgsCodeFix();
        private static readonly SortParametersCodeFix SortParametersCodeFix = new SortParametersCodeFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("GUREA13");

        private const string Condition1 = @"
namespace RoslynSandbox
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
namespace RoslynSandbox
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
namespace RoslynSandbox
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
        public void AndConditionSortArgs()
        {
            var testCode = @"
namespace RoslynSandbox
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

            var fixedCode = @"
namespace RoslynSandbox
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
            AnalyzerAssert.CodeFix(Analyzer, SortArgsCodeFix, ExpectedDiagnostic, new[] { Condition1, Condition2, testCode }, fixedCode);
        }

        [Test]
        public void OrConditionSortArgs()
        {
            var testCode = @"
namespace RoslynSandbox
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

            var fixedCode = @"
namespace RoslynSandbox
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
            AnalyzerAssert.CodeFix(Analyzer, SortArgsCodeFix, ExpectedDiagnostic, new[] { Condition1, Condition2, Condition3, testCode }, fixedCode);
        }

        [Test]
        public void AndConditionSortParameters()
        {
            var testCode = @"
namespace RoslynSandbox
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

            var fixedCode = @"
namespace RoslynSandbox
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
            AnalyzerAssert.CodeFix(Analyzer, SortParametersCodeFix, ExpectedDiagnostic, new[] { Condition1, Condition2, testCode }, fixedCode);
        }

        [Test]
        public void OrConditionSortParameters()
        {
            var testCode = @"
namespace RoslynSandbox
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

            var fixedCode = @"
namespace RoslynSandbox
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
            AnalyzerAssert.CodeFix(Analyzer, SortParametersCodeFix, ExpectedDiagnostic, new[] { Condition1, Condition2, Condition3, testCode }, fixedCode);
        }
    }
}