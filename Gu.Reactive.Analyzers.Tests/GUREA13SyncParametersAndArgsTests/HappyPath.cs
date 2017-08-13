namespace Gu.Reactive.Analyzers.Tests.GUREA13SyncParametersAndArgsTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class HappyPath
    {
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
            : base(condition1, condition2)
        {
        }
    }
}";
            AnalyzerAssert.NoDiagnostics<GUREA13SyncParametersAndArgs>(Condition1, Condition2, testCode);
        }
    }
}
