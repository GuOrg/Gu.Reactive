namespace Gu.Reactive.Analyzers.Tests.GUREA07DontNegateConditionTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class CodeFix
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly CodeFixProvider Fix = new InjectNegatedCodeFix();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(GUREA07DontNegateCondition.Descriptor);

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
        public void WhenNegatingCondition()
        {
            var conditionCode = @"
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
            var before = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class Bar
    {
        public Bar(FooCondition fooCondition)
        {
            var condition = fooCondition.↓Negate();
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class Bar
    {
        public Bar(Negated<FooCondition> notFooCondition)
        {
            var condition = notFooCondition;
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { FooCode, conditionCode, before }, after);
        }

        [Test]
        public void WhenPassingNegatedConditionToBaseCtor()
        {
            var fooConditionCode = @"
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
            var barConditionCode = @"
namespace RoslynSandbox
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class BarCondition : Condition
    {
        public BarCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value),
                () => foo.Value == 2)
        {
        }
    }
}";
            var before = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class MegaMeh : AndCondition
    {
        public MegaMeh(FooCondition fooCondition, BarCondition barCondition)
            : base(fooCondition.↓Negate(), barCondition)
        {
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class MegaMeh : AndCondition
    {
        public MegaMeh(Negated<FooCondition> notFooCondition, BarCondition barCondition)
            : base(notFooCondition, barCondition)
        {
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { FooCode, fooConditionCode, barConditionCode, before }, after);
        }

        [Test]
        public void WhenPassingNegatedConditionToBaseCtorArgPerLine()
        {
            var fooConditionCode = @"
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
            var barConditionCode = @"
namespace RoslynSandbox
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class BarCondition : Condition
    {
        public BarCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value),
                () => foo.Value == 2)
        {
        }
    }
}";
            var before = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class MegaMeh : AndCondition
    {
        public MegaMeh(
            FooCondition fooCondition,
            BarCondition barCondition)
            : base(
                fooCondition.↓Negate(),
                barCondition)
        {
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class MegaMeh : AndCondition
    {
        public MegaMeh(
            Negated<FooCondition> notFooCondition,
            BarCondition barCondition)
            : base(
                notFooCondition,
                barCondition)
        {
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { FooCode, fooConditionCode, barConditionCode, before }, after);
        }

        [Test]
        public void WhenNegatingNegatedCondition()
        {
            var fooConditionCode = @"
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
            var barConditionCode = @"
namespace RoslynSandbox
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class BarCondition : Condition
    {
        public BarCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value),
                () => foo.Value == 2)
        {
        }
    }
}";
            var before = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class MegaMeh : AndCondition
    {
        public MegaMeh(Negated<FooCondition> notFooCondition, BarCondition barCondition)
            : base(notFooCondition.↓Negate(), barCondition)
        {
        }
    }
}";

            var after = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class MegaMeh : AndCondition
    {
        public MegaMeh(FooCondition fooCondition, BarCondition barCondition)
            : base(fooCondition, barCondition)
        {
        }
    }
}";
            RoslynAssert.CodeFix(Analyzer, Fix, ExpectedDiagnostic, new[] { FooCode, fooConditionCode, barConditionCode, before }, after);
        }
    }
}
