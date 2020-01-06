namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static partial class CodeFix
    {
        public class EventHandlerOfInt
        {
            private const string FooCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler<int> SomeEvent;
    }
}";

            [Test]
            public void WhenNotUsingSenderNorArgLambda()
            {
                var before = @"
namespace RoslynSandbox
{
    using System;

    public class C2
    {
        public C2()
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

    public class C2
    {
        public C2()
        {
            var foo = new Foo();
            Observable.FromEvent<EventHandler<int>, int>(
                h => (_, e) => h(e),
                h => foo.SomeEvent += h,
                h => foo.SomeEvent -= h)
                      .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { FooCode, before }, after);
            }

            [Test]
            public void WhenUsingArgLambda()
            {
                var before = @"
namespace RoslynSandbox
{
    using System;

    public class C2
    {
        public C2()
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

    public class C2
    {
        public C2()
        {
            var foo = new Foo();
            Observable.FromEvent<EventHandler<int>, int>(
                h => (_, e) => h(e),
                h => foo.SomeEvent += h,
                h => foo.SomeEvent -= h)
                      .Subscribe(i => Console.WriteLine(i));
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { FooCode, before }, after);
            }

            [Test]
            public void WhenNotUsingSenderNorArgMethodGroup()
            {
                var before = @"
namespace RoslynSandbox
{
    using System;

    public class C2
    {
        public C2()
        {
            var foo = new Foo();
            ↓foo.SomeEvent += this.OnSomeEvent;
        }

        private void OnSomeEvent(object sender, int i)
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

    public class C2
    {
        public C2()
        {
            var foo = new Foo();
            Observable.FromEvent<EventHandler<int>, int>(
                h => (_, e) => h(e),
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
            public void WhenUsingArgMethodGroup()
            {
                var before = @"
namespace RoslynSandbox
{
    using System;

    public class C2
    {
        public C2()
        {
            var foo = new Foo();
            ↓foo.SomeEvent += this.OnSomeEvent;
        }

        private void OnSomeEvent(object sender, int i)
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

    public class C2
    {
        public C2()
        {
            var foo = new Foo();
            Observable.FromEvent<EventHandler<int>, int>(
                h => (_, e) => h(e),
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
