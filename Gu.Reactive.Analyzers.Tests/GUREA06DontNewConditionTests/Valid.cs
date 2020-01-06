namespace Gu.Reactive.Analyzers.Tests.GUREA06DontNewConditionTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ConstructorAnalyzer();

        [Test]
        public static void WhenInjectingCondition()
        {
            var fooCode = @"
namespace N
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
            var conditionCode = @"
namespace N
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
            var testCode = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class Bar
    {
        private readonly ICondition condition;
        public Bar(FooCondition condition)
        {
            var foo = new Foo();
            this.condition = condition;
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, conditionCode, testCode);
        }
    }
}
