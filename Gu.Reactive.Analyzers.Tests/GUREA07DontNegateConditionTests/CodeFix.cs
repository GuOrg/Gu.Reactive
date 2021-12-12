namespace Gu.Reactive.Analyzers.Tests.GUREA07DontNegateConditionTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class CodeFix
    {
        private static readonly InvocationAnalyzer Analyzer = new();
        private static readonly InjectNegatedCodeFix Fix = new();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA07DoNotNegateCondition);

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
        public static void WhenNegatingCondition()
        {
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
            var before = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class C
    {
        public C(ValueCondition valueCondition)
        {
            var condition = valueCondition.↓Negate();
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
        public C(Negated<ValueCondition> notValueCondition)
        {
            var condition = notValueCondition;
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { C1, valueCondition, before }, after);
        }

        [Test]
        public static void WhenPassingNegatedConditionToBaseCtor()
        {
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
            var otherCondition = @"
namespace N
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class OtherCondition : Condition
    {
        public OtherCondition(C1 c1)
            : base(
                c1.ObservePropertyChangedSlim(x => x.Value),
                () => c1.Value == 2)
        {
        }
    }
}";
            var before = @"
namespace N
{
    using Gu.Reactive;

    public class C : AndCondition
    {
        public C(ValueCondition valueCondition, OtherCondition otherCondition)
            : base(valueCondition.↓Negate(), otherCondition)
        {
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Reactive;

    public class C : AndCondition
    {
        public C(Negated<ValueCondition> notValueCondition, OtherCondition otherCondition)
            : base(notValueCondition, otherCondition)
        {
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { C1, valueCondition, otherCondition, before }, after);
        }

        [Test]
        public static void WhenPassingNegatedConditionToBaseCtorArgPerLine()
        {
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
            var otherCondition = @"
namespace N
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class OtherCondition : Condition
    {
        public OtherCondition(C1 c1)
            : base(
                c1.ObservePropertyChangedSlim(x => x.Value),
                () => c1.Value == 2)
        {
        }
    }
}";
            var before = @"
namespace N
{
    using Gu.Reactive;

    public class C : AndCondition
    {
        public C(
            ValueCondition valueCondition,
            OtherCondition otherCondition)
            : base(
                valueCondition.↓Negate(),
                otherCondition)
        {
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Reactive;

    public class C : AndCondition
    {
        public C(
            Negated<ValueCondition> notValueCondition,
            OtherCondition otherCondition)
            : base(
                notValueCondition,
                otherCondition)
        {
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { C1, valueCondition, otherCondition, before }, after);
        }

        [Test]
        public static void WhenNegatingNegatedCondition()
        {
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
            var otherCondition = @"
namespace N
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class OtherCondition : Condition
    {
        public OtherCondition(C1 c1)
            : base(
                c1.ObservePropertyChangedSlim(x => x.Value),
                () => c1.Value == 2)
        {
        }
    }
}";
            var before = @"
namespace N
{
    using Gu.Reactive;

    public class C : AndCondition
    {
        public C(Negated<ValueCondition> notValueCondition, OtherCondition otherCondition)
            : base(notValueCondition.↓Negate(), otherCondition)
        {
        }
    }
}";

            var after = @"
namespace N
{
    using Gu.Reactive;

    public class C : AndCondition
    {
        public C(ValueCondition valueCondition, OtherCondition otherCondition)
            : base(valueCondition, otherCondition)
        {
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { C1, valueCondition, otherCondition, before }, after);
        }
    }
}
