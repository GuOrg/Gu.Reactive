namespace Gu.Reactive.Analyzers.Tests.GUREA01DontObserveMutablePropertyTests
{
    using System.Threading.Tasks;
    using Gu.Reactive.Analyzers.Tests.TestHelpers;
    using Gu.Reactive.Analyzers.Tests.Verifiers;
    using NUnit.Framework;

    internal class HappyPath : HappyPathVerifier<GUREA01DontObserveMutableProperty>
    {
        [TestCase("new Foo(a, b)")]
        [TestCase("new Foo(a: a, b: b)")]
        public async Task ConstructorCallWithTwoArguments(string call)
        {
            var testCode = @"
    public class Foo
    {
        public Foo(int a, int b)
        {
            this.A = a;
            this.B = b;
        }

        public int A { get; }

        public int B { get; }

        private Foo Create(int a, int b)
        {
            return new Foo(a, b);
        }
    }";
            testCode = testCode.AssertReplace("new Foo(a, b)", call);
            await this.VerifyHappyPathAsync(testCode)
                      .ConfigureAwait(false);
        }

        [TestCase("new Foo(a, b)")]
        [TestCase("new Foo(a: a, b: b)")]
        public async Task ConstructorCallWithTwoArgumentsStruct(string call)
        {
            var testCode = @"
    public struct Foo
    {
        public Foo(int a, int b)
        {
            this.A = a;
            this.B = b;
        }

        public int A { get; }

        public int B { get; }

        private Foo Create(int a, int b)
        {
            return new Foo(a, b);
        }
    }";
            testCode = testCode.AssertReplace("new Foo(a, b)", call);
            await this.VerifyHappyPathAsync(testCode)
                      .ConfigureAwait(false);
        }

        [Test]
        public async Task ConstructorCallWithNamedArgumentsOnSameRow()
        {
            var testCode = @"
    public class Foo
    {
        public Foo(int a, int b, int c, int d)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
        }

        public int A { get; }

        public int B { get; }

        public int C { get; }

        public int D { get; }

        private Foo Create(int a, int b, int c, int d)
        {
            return new Foo(a: a, b: b, c: c, d: d);
        }
    }";

            await this.VerifyHappyPathAsync(testCode)
                      .ConfigureAwait(false);
        }

        [Test]
        public async Task ConstructorCallWithArgumentsOnSameRow()
        {
            var testCode = @"
    public class Foo
    {
        public Foo(int a, int b, int c, int d)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
        }

        public int A { get; }

        public int B { get; }

        public int C { get; }

        public int D { get; }

        private Foo Create(int a, int b, int c, int d)
        {
            return new Foo(a, b, c, d);
        }
    }";

            await this.VerifyHappyPathAsync(testCode)
                      .ConfigureAwait(false);
        }

        [Test]
        public async Task ConstructorCallWithNamedArgumentsOnSeparateRows()
        {
            var testCode = @"
    public class Foo
    {
        public Foo(int a, int b, int c, int d)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
        }

        public int A { get; }

        public int B { get; }

        public int C { get; }

        public int D { get; }

        private Foo Create(int a, int b, int c, int d)
        {
            return new Foo(
                a: a, 
                b: b, 
                c: c, 
                d: d);
        }
    }";

            await this.VerifyHappyPathAsync(testCode)
                      .ConfigureAwait(false);
        }

        [Test]
        public async Task IgnoresStringFormat()
        {
            var testCode = @"
    using System.Globalization;

    public static class Foo
    {
        private static string Bar(int a, int b, int c, int d)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                ""{0}{1}{2}{3}"",
                a,
                b,
                c,
                d);
        }
    }";

            await this.VerifyHappyPathAsync(testCode)
                      .ConfigureAwait(false);
        }

        [Test]
        public async Task IgnoresTuple()
        {
            var testCode = @"
    using System;

    public static class Foo
    {
        private static Tuple<int,int,int,int> Bar(int a, int b, int c, int d)
        {
            return Tuple.Create(
                a,
                b,
                c,
                d);
        }
    }";

            await this.VerifyHappyPathAsync(testCode)
                      .ConfigureAwait(false);
        }

        [Test]
        public async Task IgnoresParams()
        {
            var testCode = @"
    public static class Foo
    {
        public static void Bar(params int[] args)
        {
        }

        public static void Meh()
        {
            Bar(
                1,
                2,
                3,
                4,
                5,
                6);
        }
    }";

            await this.VerifyHappyPathAsync(testCode)
                      .ConfigureAwait(false);
        }

        [Test]
        public async Task IgnoresWhendifferentTypes()
        {
            var testCode = @"
    public class Foo
    {
        public Foo(int a, double b, string c, int d)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
        }

        public int A { get; }

        public double B { get; }

        public string C { get; }

        public int D { get; }

        private Foo Create(int a, double b, string c, int d)
        {
            return new Foo(
                a, 
                b, 
                c, 
                d);
        }
    }";

            await this.VerifyHappyPathAsync(testCode)
                      .ConfigureAwait(false);
        }

        [Test]
        public async Task IgnoresWhenInExpressionTree()
        {
            var testCode = @"
    using System;
    using System.Linq.Expressions;

    public class Foo
    {
        public Foo(int a, int b, int c, int d)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
        }

        public int A { get; }

        public int B { get; }

        public int C { get; }

        public int D { get; }

        private Expression<Func<Foo>> Create(int a, int b, int c, int d)
        {
            return () => new Foo(
                a,
                b,
                c,
                d);
        }
    }";

            await this.VerifyHappyPathAsync(testCode)
                      .ConfigureAwait(false);
        }
    }
}