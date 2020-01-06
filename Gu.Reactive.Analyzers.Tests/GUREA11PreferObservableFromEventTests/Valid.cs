namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new AddAssignmentAnalyzer();

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
        public static void Misc()
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
            RoslynAssert.Valid(Analyzer, FooCode, testCode);
        }

        [Test]
        public static void InsideObservableFromEventArg()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System;

    public class C1
    {
        public event EventHandler<int> SomeEvent;
    }
}";

            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Linq;

    public class C2
    {
        public C2()
        {
            var c1 = new C1();
            using (Observable.FromEvent<EventHandler<int>, int>(
                                 h => (_, e) => h(e),
                                 h => c1.SomeEvent += h,
                                 h => c1.SomeEvent -= h)
                             .Subscribe(_ => Console.WriteLine(string.Empty)))
            {
            }
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
        }
    }
}
