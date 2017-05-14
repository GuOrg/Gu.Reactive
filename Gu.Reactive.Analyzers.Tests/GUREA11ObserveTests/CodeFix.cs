namespace Gu.Reactive.Analyzers.Tests.GUREA11ObserveTests
{
    using Gu.Reactive.Analyzers.CodeFixes;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class CodeFix
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

        static CodeFix()
        {
            AnalyzerAssert.MetadataReference.AddRange(MetadataReferences.All);
        }

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
            foo.SomeEvent ↓+= (sender, i) => Console.WriteLine(string.Empty);
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
            System.Reactive.Linq.Observable.FromEvent<System.EventHandler<int>, int>(
                h => foo.SomeEvent += h,
                h => foo.SomeEvent -= h)
                                           .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            AnalyzerAssert.CodeFix<GUREA11Observe, EventSubscriptionToObserveFix>(new[] { FooCode, testCode }, fixedCode);
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
            foo.SomeEvent ↓+= (sender, i) => Console.WriteLine(i);
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
            System.Reactive.Linq.Observable.FromEvent<System.EventHandler<int>, int>(
                h => foo.SomeEvent += h,
                h => foo.SomeEvent -= h)
                                           .Subscribe(i => Console.WriteLine(i));
        }
    }
}";
            AnalyzerAssert.CodeFix<GUREA11Observe, EventSubscriptionToObserveFix>(new[] { FooCode, testCode }, fixedCode);
        }
    }
}
