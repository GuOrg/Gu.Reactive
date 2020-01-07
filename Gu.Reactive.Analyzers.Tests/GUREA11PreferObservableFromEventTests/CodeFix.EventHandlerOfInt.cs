namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static partial class CodeFix
    {
        public class EventHandlerOfInt
        {
            [Test]
            public void WhenNotUsingSenderNorArgLambda()
            {
                var c1 = @"
namespace N
{
    using System;

    public class C1
    {
        public event EventHandler<int> E;
    }
}";

                var before = @"
namespace N
{
    using System;

    public class C2
    {
        public C2()
        {
            var foo = new C1();
            ↓foo.E += (sender, i) => Console.WriteLine(string.Empty);
        }
    }
}";

                var after = @"
namespace N
{
    using System;
    using System.Reactive.Linq;

    public class C2
    {
        public C2()
        {
            var foo = new C1();
            Observable.FromEvent<EventHandler<int>, int>(
                h => (_, e) => h(e),
                h => foo.E += h,
                h => foo.E -= h)
                      .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { c1, before }, after);
            }

            [Test]
            public void WhenUsingArgLambda()
            {
                var c1 = @"
namespace N
{
    using System;

    public class C1
    {
        public event EventHandler<int> E;
    }
}";

                var before = @"
namespace N
{
    using System;

    public class C2
    {
        public C2()
        {
            var foo = new C1();
            ↓foo.E += (sender, i) => Console.WriteLine(i);
        }
    }
}";

                var after = @"
namespace N
{
    using System;
    using System.Reactive.Linq;

    public class C2
    {
        public C2()
        {
            var foo = new C1();
            Observable.FromEvent<EventHandler<int>, int>(
                h => (_, e) => h(e),
                h => foo.E += h,
                h => foo.E -= h)
                      .Subscribe(i => Console.WriteLine(i));
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { c1, before }, after);
            }

            [Test]
            public void WhenNotUsingSenderNorArgMethodGroup()
            {
                var c1 = @"
namespace N
{
    using System;

    public class C1
    {
        public event EventHandler<int> E;
    }
}";

                var before = @"
namespace N
{
    using System;

    public class C2
    {
        public C2()
        {
            var foo = new C1();
            ↓foo.E += this.OnE;
        }

        private void OnE(object sender, int i)
        {
            Console.WriteLine(string.Empty);
        }
    }
}";

                var after = @"
namespace N
{
    using System;
    using System.Reactive.Linq;

    public class C2
    {
        public C2()
        {
            var foo = new C1();
            Observable.FromEvent<EventHandler<int>, int>(
                h => (_, e) => h(e),
                h => foo.E += h,
                h => foo.E -= h)
                      .Subscribe(_ => OnE());
        }

        private void OnE()
        {
            Console.WriteLine(string.Empty);
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { c1, before }, after);
            }

            [Test]
            public void WhenUsingArgMethodGroup()
            {
                var c1 = @"
namespace N
{
    using System;

    public class C1
    {
        public event EventHandler<int> E;
    }
}";

                var before = @"
namespace N
{
    using System;

    public class C2
    {
        public C2()
        {
            var foo = new C1();
            ↓foo.E += this.OnE;
        }

        private void OnE(object sender, int i)
        {
            Console.WriteLine(i);
        }
    }
}";

                var after = @"
namespace N
{
    using System;
    using System.Reactive.Linq;

    public class C2
    {
        public C2()
        {
            var foo = new C1();
            Observable.FromEvent<EventHandler<int>, int>(
                h => (_, e) => h(e),
                h => foo.E += h,
                h => foo.E -= h)
                      .Subscribe(OnE);
        }

        private void OnE(int i)
        {
            Console.WriteLine(i);
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { c1, before }, after);
            }
        }
    }
}
