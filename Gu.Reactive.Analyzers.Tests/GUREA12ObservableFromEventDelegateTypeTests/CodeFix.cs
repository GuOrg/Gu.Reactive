namespace Gu.Reactive.Analyzers.Tests.GUREA12ObservableFromEventDelegateTypeTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly InvocationAnalyzer Analyzer = new();
        private static readonly ObservableFromEventArgsFix Fix = new();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = Roslyn.Asserts.ExpectedDiagnostic.Create(Descriptors.GUREA12ObservableFromEventDelegateType);

        [Test]
        public static void EventHandlerOfInt()
        {
            var c1 = @"
namespace N
{
    using System;

    public class C1
    {
        public event EventHandler<int> SomeEvent;
    }
}";

            var before = @"
namespace N
{
    using System;

    public class C
    {
        public C()
        {
            var c1 = new C1();
            ↓System.Reactive.Linq.Observable.FromEvent<System.EventHandler<int>, int>(
                h => c1.SomeEvent += h,
                h => c1.SomeEvent -= h)
                                           .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";

            var after = @"
namespace N
{
    using System;

    public class C
    {
        public C()
        {
            var c1 = new C1();
            System.Reactive.Linq.Observable.FromEvent<System.EventHandler<int>, int>(
                h => (_, e) => h(e), h => c1.SomeEvent += h,
                h => c1.SomeEvent -= h)
                                           .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { c1, before }, after);
        }

        [Test]
        public static void EventHandler()
        {
            var c1 = @"
namespace N
{
    using System;

    public class C1
    {
        public event EventHandler SomeEvent;
    }
}";

            var before = @"
namespace N
{
    using System;

    public class C
    {
        public C()
        {
            var c1 = new C1();
            ↓System.Reactive.Linq.Observable.FromEvent<System.EventHandler, EventArgs>(
                h => c1.SomeEvent += h,
                h => c1.SomeEvent -= h)
                                           .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";

            var after = @"
namespace N
{
    using System;

    public class C
    {
        public C()
        {
            var c1 = new C1();
            System.Reactive.Linq.Observable.FromEvent<System.EventHandler, EventArgs>(
                h => (_, e) => h(e), h => c1.SomeEvent += h,
                h => c1.SomeEvent -= h)
                                           .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { c1, before }, after);
        }
    }
}
