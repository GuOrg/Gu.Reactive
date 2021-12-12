namespace Gu.Reactive.Analyzers.Tests.GUREA09ObservableBeforeCriteriaTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly ConstructorAnalyzer Analyzer = new();
        private static readonly ObservableBeforeCriteriaCodeFix Fix = new();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA09ObservableBeforeCriteria);

        private const string C1 = @"
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

        [Test]
        public static void BaseCall()
        {
            var before = @"
namespace N
{
    using Gu.Reactive;

    public class ValueCondition : Condition
    {
        public ValueCondition(C1 c1)
            : base↓(
                () => c1.Value == 2,
                c1.ObservePropertyChangedSlim(x => x.Value))
        {
        }
    }
}";

            var after = @"
namespace N
{
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
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { C1, before }, after);
        }

        [Test]
        public static void New()
        {
            var before = @"
namespace N
{
    using Gu.Reactive;

    class C
    {
        public static ICondition Create()
        {
            var c1 = new C1();
            return new Condition↓(
                () => c1.Value == 2,
                c1.ObservePropertyChangedSlim(x => x.Value));
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Reactive;

    class C
    {
        public static ICondition Create()
        {
            var c1 = new C1();
            return new Condition(
                c1.ObservePropertyChangedSlim(x => x.Value),
                () => c1.Value == 2);
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic,  new[] { C1, before }, after);
        }
    }
}
