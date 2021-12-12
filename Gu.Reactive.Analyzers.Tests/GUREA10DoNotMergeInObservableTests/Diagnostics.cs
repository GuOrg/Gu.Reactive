namespace Gu.Reactive.Analyzers.Tests.GUREA10DoNotMergeInObservableTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly ConstructorAnalyzer Analyzer = new();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA10DoNotMergeInObservable);

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
        public static void WhenMergingInline()
        {
            var code = @"
namespace N
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class P1AndP2Condition : Condition
    {
        public P1AndP2Condition(C1 c1)
            : base(
               ↓Observable.Merge(
                   c1.ObservePropertyChangedSlim(x => x.P1),
                   c1.ObservePropertyChangedSlim(x => x.P2)),
               () => c1.P1 == 1 && c1.P2 == 2)
        {
        }
    }
}";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, C1, code);
        }

        [Test]
        public static void WhenMergingInFactoryMethod()
        {
            var code = @"
namespace N
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class P1AndP2Condition : Condition
    {
        public P1AndP2Condition(C1 c1)
            : base(
               ↓CreateObservable(c1),
               () => c1.P1 == 1 && c1.P2 == 2)
        {
        }

        private static IObservable<PropertyChangedEventArgs> CreateObservable(C1 c1)
        {
            return Observable.Merge(
                c1.ObservePropertyChangedSlim(x => x.P1),
                c1.ObservePropertyChangedSlim(x => x.P2));
        }
    }
}";

            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, C1, code);
        }
    }
}
