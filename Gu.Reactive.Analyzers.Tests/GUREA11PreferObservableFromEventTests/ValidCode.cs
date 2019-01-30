namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class ValidCode
    {
        private static readonly DiagnosticAnalyzer Analyzer = new GUREA11PreferObservableFromEvent();

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
        public void Misc()
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
            AnalyzerAssert.Valid(Analyzer, FooCode, testCode);
        }

        [Test]
        public void InsideObservableFromEventArg()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System;

    public class Foo
    {
        public event EventHandler<int> SomeEvent;
    }
}";

            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            using (Observable.FromEvent<EventHandler<int>, int>(
                                 h => (_, e) => h(e),
                                 h => foo.SomeEvent += h,
                                 h => foo.SomeEvent -= h)
                             .Subscribe(_ => Console.WriteLine(string.Empty)))
            {
            }
        }
    }
}";
            AnalyzerAssert.Valid(Analyzer, fooCode, testCode);
        }
    }
}
