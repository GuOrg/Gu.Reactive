namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static partial class CodeFix
    {
        public static class ActionOfInt
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
            public static void WhenNotUsingSenderNorArgLambda()
            {
                var before = @"
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

                var after = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            Observable.FromEvent<Action<int>, int>(
                h => foo.SomeEvent += h,
                h => foo.SomeEvent -= h)
                      .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";

                RoslynAssert.CodeFix(Analyzer, Fix, new[] { FooCode, before }, after);
            }

            [Test]
            public static void WhenUsingArgLambda()
            {
                var before = @"
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

                var after = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            Observable.FromEvent<Action<int>, int>(
                h => foo.SomeEvent += h,
                h => foo.SomeEvent -= h)
                      .Subscribe(i => Console.WriteLine(i));
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { FooCode, before }, after);
            }

            [Test]
            public static void WhenNotUsingSenderNorArgMethodGroup()
            {
                var before = @"
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

                var after = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            Observable.FromEvent<Action<int>, int>(
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
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { FooCode, before }, after);
            }

            [Test]
            public static void WhenUsingArgMethodGroup()
            {
                var before = @"
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

                var after = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            Observable.FromEvent<Action<int>, int>(
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
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { FooCode, before }, after);
            }
        }
    }
}
