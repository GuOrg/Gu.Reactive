namespace Gu.Reactive.Analyzers.Tests.GUREA08InlineSingleLineTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly ConstructorAnalyzer Analyzer = new();
        private static readonly InlineSingleLineCodeFix Fix = new();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA08InlineSingleLine);

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
        public static void WhenCreateObservableIsSingleLine()
        {
            var before = @"
namespace N
{
    using System;
    using System.ComponentModel;
    using Gu.Reactive;

    public class ValueCondition : Condition
    {
        public ValueCondition(C1 c1)
            : base(
                ↓CreateObservable(c1),
                () => c1.Value == 2)
        {
        }

        private static IObservable<PropertyChangedEventArgs> CreateObservable(C1 arg)
        {
            return arg.ObservePropertyChangedSlim(x => x.Value);
        }
    }
}";

            var after = @"
namespace N
{
    using System;
    using System.ComponentModel;
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
        public static void WhenCriteriaIsSingleLineStatementBody()
        {
            var before = @"
namespace N
{
    using Gu.Reactive;

    public class ValueCondition : Condition
    {
        public ValueCondition(C1 c1)
            : base(
                c1.ObservePropertyChangedSlim(x => x.Value),
                () => ↓Criteria(c1))
        {
        }

        private static bool Criteria(C1 arg)
        {
            return arg.Value == 2;
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
        public static void WhenCriteriaIsSingleLineExpressionBody()
        {
            var before = @"
namespace N
{
    using Gu.Reactive;

    public class ValueCondition : Condition
    {
        public ValueCondition(C1 c1)
            : base(
                c1.ObservePropertyChangedSlim(x => x.Value),
                () => ↓Criteria(c1))
        {
        }

        private static bool Criteria(C1 arg) => arg.Value == 2;
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
    }
}
