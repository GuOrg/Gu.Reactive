namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Reactive.Analyzers.CodeFixes;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public partial class CodeFix
    {
        public class ActionOfInt
        {
            private const string FooCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event Action<int> SomeEvent;
    }
}";

            [Test]
            public void WhenNotUsingSenderNorArgLambda()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            ↓foo.SomeEvent += (sender, i) => Console.WriteLine(string.Empty);
        }
    }
}";

                var fixedCode = @"
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
                AnalyzerAssert.CodeFix<GUREA11PreferObservableFromEvent, EventSubscriptionToObserveFix>(new[] { FooCode, testCode }, fixedCode);
            }

            [Test]
            public void WhenUsingArgLambda()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            ↓foo.SomeEvent += (sender, i) => Console.WriteLine(i);
        }
    }
}";

                var fixedCode = @"
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
                                           .Subscribe(i => Console.WriteLine(i));
        }
    }
}";
                AnalyzerAssert.CodeFix<GUREA11PreferObservableFromEvent, EventSubscriptionToObserveFix>(new[] { FooCode, testCode }, fixedCode);
            }

            [Test]
            public void WhenNotUsingSenderNorArgMethodGroup()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            ↓foo.SomeEvent += this.OnSomeEvent;
        }

        private void OnSomeEvent(int i)
        {
            Console.WriteLine(string.Empty);
        }
    }
}";

                var fixedCode = @"
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
                                           .Subscribe(_ => OnSomeEvent());
        }

        private void OnSomeEvent()
        {
            Console.WriteLine(string.Empty);
        }
    }
}";
                AnalyzerAssert.CodeFix<GUREA11PreferObservableFromEvent, EventSubscriptionToObserveFix>(new[] { FooCode, testCode }, fixedCode);
            }

            [Test]
            public void WhenUsingArgMethodGroup()
            {
                var testCode = @"
namespace RoslynSandbox
{
    using System;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            ↓foo.SomeEvent += this.OnSomeEvent;
        }

        private void OnSomeEvent(int i)
        {
            Console.WriteLine(i);
        }
    }
}";

                var fixedCode = @"
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
                                           .Subscribe(OnSomeEvent);
        }

        private void OnSomeEvent(int i)
        {
            Console.WriteLine(i);
        }
    }
}";
                AnalyzerAssert.CodeFix<GUREA11PreferObservableFromEvent, EventSubscriptionToObserveFix>(new[] { FooCode, testCode }, fixedCode);
            }
        }
    }
}