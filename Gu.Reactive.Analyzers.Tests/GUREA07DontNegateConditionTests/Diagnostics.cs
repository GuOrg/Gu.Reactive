namespace Gu.Reactive.Analyzers.Tests.GUREA07DontNegateConditionTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class Diagnostics
    {
        static Diagnostics()
        {
            AnalyzerAssert.MetadataReference.AddRange(MetadataReferences.All);
        }

        [Test]
        public void WhenNegatingCondition()
        {
            var fooCode = @"
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
            var conditionCode = @"
namespace RoslynSandbox
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                Observable.Merge(
                    foo.ObservePropertyChangedSlim(x => x.Value1),
                    foo.ObservePropertyChangedSlim(x => x.Value1)),
                () => true)
        {
        }
    }
}";
            var testCode = @"
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
            AnalyzerAssert.Diagnostics<GUREA07DontNegateCondition>(fooCode, conditionCode, testCode);
        }
    }
}
