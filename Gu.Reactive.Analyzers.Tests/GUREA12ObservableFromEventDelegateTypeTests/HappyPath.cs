namespace Gu.Reactive.Analyzers.Tests.GUREA12ObservableFromEventDelegateTypeTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class HappyPath
    {
        private static readonly GUREA12ObservableFromEventDelegateType Analyzer = new GUREA12ObservableFromEventDelegateType();

        [Test]
        public void ActionOfInt()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event Action<int> SomeEvent;
    }
}";

            var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            System.Reactive.Linq.Observable.FromEvent<System.Action<int>, int>(
                h => foo.SomeEvent += h,
                h => foo.SomeEvent -= h)
                                           .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, fooCode, testCode);
        }

        [Test]
        public void EventHandlerOfInt()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler<int> SomeEvent;
    }
}";

            var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            System.Reactive.Linq.Observable.FromEvent<System.EventHandler<int>, int>(
                h => (_, e) => h(e),
                h => foo.SomeEvent += h,
                h => foo.SomeEvent -= h)
                                           .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, fooCode, testCode);
        }

        [Test]
        public void EventHandler()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler SomeEvent;
    }
}";

            var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            System.Reactive.Linq.Observable.FromEvent<System.EventHandler, EventArgs>(
                h => (_, e) => h(e),
                h => foo.SomeEvent += h,
                h => foo.SomeEvent -= h)
                                           .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, fooCode, testCode);
        }
    }
}
