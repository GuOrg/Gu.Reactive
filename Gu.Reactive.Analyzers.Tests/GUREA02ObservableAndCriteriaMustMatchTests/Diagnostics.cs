namespace Gu.Reactive.Analyzers.Tests.GUREA02ObservableAndCriteriaMustMatchTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly ConstructorAnalyzer Analyzer = new();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA02ObservableAndCriteriaMustMatch);

        private const string C1 = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C1 : INotifyPropertyChanged
    {
        private int p1;
        private int p2;

        public event PropertyChangedEventHandler PropertyChanged;

        public int P1
        {
            get
            {
                return this.p1;
            }

            set
            {
                if (value == this.p1)
                {
                    return;
                }

                this.p1 = value;
                this.OnPropertyChanged();
            }
        }

        public int P2
        {
            get
            {
                return this.p2;
            }

            set
            {
                if (value == this.p2)
                {
                    return;
                }

                this.p2 = value;
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
            var code = @"
namespace N
{
    using Gu.Reactive;

    public class C1Condition : Condition
    {
        public C1Condition(C1 C1)
            ↓: base(
                C1.ObservePropertyChangedSlim(x => x.P1),
                () => C1.P2 == 2)
        {
        }
    }
}";
            var message = "Observable and criteria must match.\r\n" +
                           "Observed:\r\n" +
                           "  N.C1.P1\r\n" +
                           "Used in criteria:\r\n" +
                           "  N.C1.P2\r\n" +
                           "Not observed:\r\n" +
                           "  N.C1.P2";
            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic.WithMessage(message), C1, code);
        }

        [Test]
        public static void New()
        {
            var code = @"
namespace N
{
    using Gu.Reactive;

    class C2
    {
        public static ICondition Create()
        {
            var C1 = new C1();
            return ↓new Condition(
                C1.ObservePropertyChangedSlim(x => x.P1),
                () => C1.P2 == 2);
        }
    }
}";
            var message = "Observable and criteria must match.\r\n" +
                          "Observed:\r\n" +
                          "  N.C1.P1\r\n" +
                          "Used in criteria:\r\n" +
                          "  N.C1.P2\r\n" +
                          "Not observed:\r\n" +
                          "  N.C1.P2";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic.WithMessage(message), C1, code);
        }
    }
}
