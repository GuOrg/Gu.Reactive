namespace Gu.Reactive.Analyzers.Tests.GUREA08InlineSingleLineTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ConstructorAnalyzer();
        private static readonly CodeFixProvider Fix = new InlineSingleLineCodeFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA08InlineSingleLine);

        private const string FooCode = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Foo : INotifyPropertyChanged
    {
        private int value;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (value == this.value)
                {
                    return;
                }

                this.value = value;
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
        public static void WhenCreateObservableIsSingleLine()
        {
            var before = @"
namespace N
{
    using System;
    using System.ComponentModel;
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                ↓CreateObservable(foo),
                () => foo.Value == 2)
        {
        }

        private static IObservable<PropertyChangedEventArgs> CreateObservable(Foo arg)
        {
            return arg.ObservePropertyChangedSlim(x => x.Value);
        }
    }
}";

            var after = @"
namespace N
{
    using System;
    using System.ComponentModel;
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value),
                () => foo.Value == 2)
        {
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { FooCode, before }, after);
        }

        [Test]
        public static void WhenCriteriaIsSingleLineStatementBody()
        {
            var before = @"
namespace N
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value),
                () => ↓Criteria(foo))
        {
        }

        private static bool Criteria(Foo arg)
        {
            return arg.Value == 2;
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value),
                () => foo.Value == 2)
        {
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { FooCode, before }, after);
        }

        [Test]
        public static void WhenCriteriaIsSingleLineExpressionBody()
        {
            var before = @"
namespace N
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value),
                () => ↓Criteria(foo))
        {
        }

        private static bool Criteria(Foo arg) => arg.Value == 2;
    }
}";

            var after = @"
namespace N
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value),
                () => foo.Value == 2)
        {
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { FooCode, before }, after);
        }
    }
}
