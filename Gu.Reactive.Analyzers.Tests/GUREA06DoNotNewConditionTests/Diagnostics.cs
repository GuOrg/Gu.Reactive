namespace Gu.Reactive.Analyzers.Tests.GUREA06DoNotNewConditionTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ConstructorAnalyzer();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA06DoNotNewCondition);

        [Test]
        public static void WhenCreatingCondition()
        {
            var c1 = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C1 : INotifyPropertyChanged
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
            var valueCondition = @"
namespace N
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class ValueCondition : Condition
    {
        public ValueCondition(C1 c1)
            : base(
                c1.ObservePropertyChangedSlim(x => x.Value),
                () => c1.Value == 2)
        {
        }
    }
}";
            var code = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class C
    {
        public C()
        {
            var c1 = new C1();
            var condition = ↓new ValueCondition(c1);
        }
    }
}";
            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, c1, valueCondition, code);
        }
    }
}
