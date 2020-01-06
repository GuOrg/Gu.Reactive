namespace Gu.Reactive.Analyzers.Tests.GUREA12ObservableFromEventDelegateTypeTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();

        [Test]
        public static void ActionOfInt()
        {
            var fooCode = @"
namespace N
{
    using System;

    public class C1
    {
        public event Action<int> SomeEvent;
    }
}";

            var testCode = @"
namespace N
{
    using System;

    public class C2
    {
        public C2()
        {
            var c1 = new C1();
            System.Reactive.Linq.Observable.FromEvent<System.Action<int>, int>(
                h => c1.SomeEvent += h,
                h => c1.SomeEvent -= h)
                                           .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
        }

        [Test]
        public static void EventHandlerOfInt()
        {
            var fooCode = @"
namespace N
{
    using System;

    public class C1
    {
        public event EventHandler<int> SomeEvent;
    }
}";

            var testCode = @"
namespace N
{
    using System;

    public class C2
    {
        public C2()
        {
            var c1 = new C1();
            System.Reactive.Linq.Observable.FromEvent<System.EventHandler<int>, int>(
                h => (_, e) => h(e),
                h => c1.SomeEvent += h,
                h => c1.SomeEvent -= h)
                                           .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
        }

        [Test]
        public static void EventHandler()
        {
            var fooCode = @"
namespace N
{
    using System;

    public class C1
    {
        public event EventHandler SomeEvent;
    }
}";

            var testCode = @"
namespace N
{
    using System;

    public class C2
    {
        public C2()
        {
            var c1 = new C1();
            System.Reactive.Linq.Observable.FromEvent<System.EventHandler, EventArgs>(
                h => (_, e) => h(e),
                h => c1.SomeEvent += h,
                h => c1.SomeEvent -= h)
                                           .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
        }
    }
}
