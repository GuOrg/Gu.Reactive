namespace Gu.Reactive.Analyzers.Tests.GUREA11PreferObservableFromEventTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new AddAssignmentAnalyzer();

        [Test]
        public static void Misc()
        {
            var c = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C : INotifyPropertyChanged
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

            var code = @"
namespace N
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class ValueCondition : Condition
    {
        public ValueCondition(C c)
            : base(
                c.ObservePropertyChangedSlim(x => x.Value),
                () => c.Value == 2)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, c, code);
        }

        [Test]
        public static void InsideObservableFromEventArg()
        {
            var c1 = @"
namespace N
{
    using System;

    public class C1
    {
        public event EventHandler<int> E;
    }
}";

            var code = @"
namespace N
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
                                 h => c1.E += h,
                                 h => c1.E -= h)
                             .Subscribe(_ => Console.WriteLine(string.Empty)))
            {
            }
        }
    }
}";
            RoslynAssert.Valid(Analyzer, c1, code);
        }
    }
}
