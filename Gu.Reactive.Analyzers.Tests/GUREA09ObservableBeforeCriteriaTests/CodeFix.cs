namespace Gu.Reactive.Analyzers.Tests.GUREA09ObservableBeforeCriteriaTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ConstructorAnalyzer();
        private static readonly CodeFixProvider Fix = new ObservableBeforeCriteriaCodeFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA09ObservableBeforeCriteria);

        private const string FooCode = @"
namespace RoslynSandbox
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
        public static void BaseCall()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base↓(
                () => foo.Value == 2,
                foo.ObservePropertyChangedSlim(x => x.Value))
        {
        }
    }
}";

            var after = @"
namespace RoslynSandbox
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
        public static void New()
        {
            var before = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    class Bar
    {
        public static ICondition Create()
        {
            var foo = new Foo();
            return new Condition↓(
                () => foo.Value == 2,
                foo.ObservePropertyChangedSlim(x => x.Value));
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    class Bar
    {
        public static ICondition Create()
        {
            var foo = new Foo();
            return new Condition(
                foo.ObservePropertyChangedSlim(x => x.Value),
                () => foo.Value == 2);
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic,  new[] { FooCode, before }, after);
        }
    }
}
