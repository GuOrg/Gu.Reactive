namespace Gu.Reactive.Analyzers.Tests.GUREA09ObservableBeforeCriteriaTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class ValidCode
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ConstructorAnalyzer();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("GUREA09");

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
        public void BaseCall()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System.Reactive.Linq;
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
            AnalyzerAssert.Valid(Analyzer, ExpectedDiagnostic, FooCode, testCode);
        }

        [Test]
        public void CorrectNew()
        {
            var testCode = @"
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
            AnalyzerAssert.Valid(Analyzer, ExpectedDiagnostic, FooCode, testCode);
        }
    }
}
