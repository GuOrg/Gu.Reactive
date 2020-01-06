namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static partial class CodeFix
    {
        public static class EventHandler
        {
            private const string C = @"
namespace RoslynSandbox
{
    using System;

    public class C
    {
        public event EventHandler E;
    }
}";

            [Test]
            public static void WhenNotUsingSenderNorArgLambda()
            {
                var before = @"
namespace RoslynSandbox
{
    using System;

    public class C1
    {
        public C1()
        {
            var C = new C();
            ↓C.E += (sender, i) => Console.WriteLine(string.Empty);
        }
    }
}";

                var after = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class C1
    {
        public C1()
        {
            var C = new C();
            Observable.FromEvent<EventHandler, EventArgs>(
                h => (_, e) => h(e),
                h => C.E += h,
                h => C.E -= h)
                      .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { C, before }, after);
            }

            [Test]
            public static void WhenUsingArgLambda()
            {
                var before = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class C1
    {
        public C1()
        {
            var C = new C();
            ↓C.E += (sender, e) => Console.WriteLine(e);
        }
    }
}";

                var after = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class C1
    {
        public C1()
        {
            var C = new C();
            Observable.FromEvent<EventHandler, EventArgs>(
                h => (_, e) => h(e),
                h => C.E += h,
                h => C.E -= h)
                      .Subscribe(e => Console.WriteLine(e));
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { C, before }, after);
            }

            [Test]
            public static void WhenNotUsingSenderNorArgMethodGroup()
            {
                var before = @"
namespace RoslynSandbox
{
    using System;

    public class C1
    {
        public C1()
        {
            var C = new C();
            ↓C.E += this.OnE;
        }

        private void OnE(object sender, EventArgs e)
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

    public class C1
    {
        public C1()
        {
            var C = new C();
            Observable.FromEvent<EventHandler, EventArgs>(
                h => (_, e) => h(e),
                h => C.E += h,
                h => C.E -= h)
                      .Subscribe(_ => OnE());
        }

        private void OnE()
        {
            Console.WriteLine(string.Empty);
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { C, before }, after);
            }

            [Test]
            public static void WhenUsingArgMethodGroup()
            {
                var before = @"
namespace RoslynSandbox
{
    using System;

    public class C1
    {
        public C1()
        {
            var C = new C();
            ↓C.E += this.OnE;
        }

        private void OnE(object sender, EventArgs e)
        {
            Console.WriteLine(e);
        }
    }
}";

                var after = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class C1
    {
        public C1()
        {
            var C = new C();
            Observable.FromEvent<EventHandler, EventArgs>(
                h => (_, e) => h(e),
                h => C.E += h,
                h => C.E -= h)
                      .Subscribe(OnE);
        }

        private void OnE(EventArgs e)
        {
            Console.WriteLine(e);
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { C, before }, after);
            }
        }
    }
}
