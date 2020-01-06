namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static partial class CodeFix
    {
        public static class ActionOfInt
        {
            private const string CCode = @"
namespace N
{
    using System;

    public class C
    {
        public event Action<int> E;
    }
}";

            [Test]
            public static void WhenNotUsingSenderNorArgLambda()
            {
                var before = @"
namespace N
{
    using System;

    public class C2
    {
        public C2()
        {
            var C = new C();
            ↓C.E += (sender, i) => Console.WriteLine(string.Empty);
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
            var C = new C();
            Observable.FromEvent<Action<int>, int>(
                h => C.E += h,
                h => C.E -= h)
                      .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";

                RoslynAssert.CodeFix(Analyzer, Fix, new[] { CCode, before }, after);
            }

            [Test]
            public static void WhenUsingArgLambda()
            {
                var before = @"
namespace N
{
    using System;

    public class C2
    {
        public C2()
        {
            var C = new C();
            ↓C.E += (sender, i) => Console.WriteLine(i);
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
            var C = new C();
            Observable.FromEvent<Action<int>, int>(
                h => C.E += h,
                h => C.E -= h)
                      .Subscribe(i => Console.WriteLine(i));
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { CCode, before }, after);
            }

            [Test]
            public static void WhenNotUsingSenderNorArgMethodGroup()
            {
                var before = @"
namespace N
{
    using System;

    public class C2
    {
        public C2()
        {
            var C = new C();
            ↓C.E += this.OnE;
        }

        private void OnE(int i)
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
            var C = new C();
            Observable.FromEvent<Action<int>, int>(
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
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { CCode, before }, after);
            }

            [Test]
            public static void WhenUsingArgMethodGroup()
            {
                var before = @"
namespace N
{
    using System;

    public class C2
    {
        public C2()
        {
            var C = new C();
            ↓C.E += this.OnE;
        }

        private void OnE(int i)
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
            var C = new C();
            Observable.FromEvent<Action<int>, int>(
                h => C.E += h,
                h => C.E -= h)
                      .Subscribe(OnE);
        }

        private void OnE(int i)
        {
            Console.WriteLine(i);
        }
    }
}";
                RoslynAssert.CodeFix(Analyzer, Fix, new[] { CCode, before }, after);
            }
        }
    }
}
