namespace Gu.Reactive.Analyzers.Tests.GUREA02ObservableAndCriteriaMustMatchTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class Diagnostics
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ConstructorAnalyzer();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create("GUREA02");

        private const string FooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Foo : INotifyPropertyChanged
    {
        private int value1;
        private int value2;

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

        public int Value2
        {
            get
            {
                return this.value2;
            }

            set
            {
                if (value == this.value2)
                {
                    return;
                }

                this.value2 = value;
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
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            ↓: base(
                foo.ObservePropertyChangedSlim(x => x.Value1),
                () => foo.Value2 == 2)
        {
        }
    }
}";
            var message = "Observable and criteria must match.\r\n" +
                           "Observed:\r\n" +
                           "  RoslynSandbox.Foo.Value1\r\n" +
                           "Used in criteria:\r\n" +
                           "  RoslynSandbox.Foo.Value2\r\n" +
                           "Not observed:\r\n" +
                           "  RoslynSandbox.Foo.Value2";
            var expectedDiagnostic = ExpectedDiagnostic.Create("GUREA02", message);
            AnalyzerAssert.Diagnostics(Analyzer, expectedDiagnostic, FooCode, testCode);
        }

        [Test]
        public void New()
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
            return ↓new Condition(
                foo.ObservePropertyChangedSlim(x => x.Value1),
                () => foo.Value2 == 2);
        }
    }
}";
            var message = "Observable and criteria must match.\r\n" +
                          "Observed:\r\n" +
                          "  RoslynSandbox.Foo.Value1\r\n" +
                          "Used in criteria:\r\n" +
                          "  RoslynSandbox.Foo.Value2\r\n" +
                          "Not observed:\r\n" +
                          "  RoslynSandbox.Foo.Value2";
            var expectedDiagnostic = ExpectedDiagnostic.Create("GUREA02", message);
            AnalyzerAssert.Diagnostics(Analyzer, expectedDiagnostic, FooCode, testCode);
        }
    }
}
