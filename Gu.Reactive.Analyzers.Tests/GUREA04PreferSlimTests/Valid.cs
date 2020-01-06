namespace Gu.Reactive.Analyzers.Tests.GUREA04PreferSlimTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();

        private const string FooCode = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Foo : INotifyPropertyChanged
    {
        private int value1;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value1
        {
            get
            {
                return this.value1;
            }

            set
            {
                if (value == this.value1)
                {
                    return;
                }

                this.value1 = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

        [Test]
        public static void WhenPassingSlimToConditionCtor()
        {
            var testCode = @"
namespace N
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value1),
                () => foo.Value1 == 2)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, FooCode, testCode);
        }

        [Test]
        public static void WhenSubscribingToSlimNotUsingArg()
        {
            var testCode = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            foo.ObservePropertyChangedSlim(x => x.Value1)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, FooCode, testCode);
        }

        [Test]
        public static void WhenSubscribingToSlimUsingArg()
        {
            var testCode = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            foo.ObservePropertyChangedSlim(x => x.Value1)
               .Subscribe(x => Console.WriteLine(x.PropertyName));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, FooCode, testCode);
        }
    }
}
