namespace Gu.Reactive.Analyzers.Tests.GUREA12ObservableFromEventDelegateTypeTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly CodeFixProvider Fix = new ObservableFromEventArgsFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = Roslyn.Asserts.ExpectedDiagnostic.Create(Descriptors.GUREA12ObservableFromEventDelegateType);

        [Test]
        public static void EventHandlerOfInt()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System;

    public class C1
    {
        public event EventHandler<int> SomeEvent;
    }
}";

            var before = @"
namespace RoslynSandbox
{
    using System;

    public class C2
    {
        public C2()
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
namespace RoslynSandbox
{
    using System;

    public class C2
    {
        public C2()
        {
            var c1 = new C1();
            System.Reactive.Linq.Observable.FromEvent<System.EventHandler<int>, int>(
                h => (_, e) => h(e), h => c1.SomeEvent += h,
                h => c1.SomeEvent -= h)
                                           .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { fooCode, before }, after);
        }

        [Test]
        public static void EventHandler()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System;

    public class C1
    {
        public event EventHandler SomeEvent;
    }
}";

            var before = @"
namespace RoslynSandbox
{
    using System;

    public class C2
    {
        public C2()
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
namespace RoslynSandbox
{
    using System;

    public class C2
    {
        public C2()
        {
            var c1 = new C1();
            System.Reactive.Linq.Observable.FromEvent<System.EventHandler, EventArgs>(
                h => (_, e) => h(e), h => c1.SomeEvent += h,
                h => c1.SomeEvent -= h)
                                           .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { fooCode, before }, after);
        }
    }
}
