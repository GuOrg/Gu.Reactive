namespace Gu.Reactive.Analyzers.Tests.GUREA04PreferSlimTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly InvocationAnalyzer Analyzer = new();
        private static readonly UseSlimFix Fix = new();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA04PreferSlimOverload);

        private const string C1 = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C1 : INotifyPropertyChanged
    {
        private int p;

        public event PropertyChangedEventHandler PropertyChanged;

        public int P
        {
            get
            {
                return this.p;
            }

            set
            {
                if (value == this.p)
                {
                    return;
                }

                this.p = value;
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
        public static void PassingObservePropertyChangedToConditionCtor()
        {
            var before = @"
namespace N
{
    using Gu.Reactive;

    public class PCondition : Condition
    {
        public PCondition(C1 c1)
            : base(
                c1.↓ObservePropertyChanged(x => x.P),
                () => c1.P == 2)
        {
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Reactive;

    public class PCondition : Condition
    {
        public PCondition(C1 c1)
            : base(
                c1.ObservePropertyChangedSlim(x => x.P),
                () => c1.P == 2)
        {
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { C1, before }, after);
        }

        [Test]
        public static void WhenSubscribingNotUsingArg()
        {
            var before = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class C
    {
        public C()
        {
            var c1 = new C1();
            c1.↓ObservePropertyChanged(x => x.P)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";

            var after = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class C
    {
        public C()
        {
            var c1 = new C1();
            c1.ObservePropertyChangedSlim(x => x.P)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { C1, before }, after);
        }
    }
}
