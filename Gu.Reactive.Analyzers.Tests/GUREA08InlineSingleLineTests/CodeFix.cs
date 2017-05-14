namespace Gu.Reactive.Analyzers.Tests.GUREA08InlineSingleLineTests
{
    using Gu.Reactive.Analyzers.CodeFixes;
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public class CodeFix
    {
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

        static CodeFix()
        {
            AnalyzerAssert.MetadataReference.AddRange(MetadataReferences.All);
        }

        [Test]
        public void WhenCreateObservableIsSingleLine()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using System.ComponentModel;
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                ↓CreateObservable(foo),
                () => foo.Value == 2)
        {
        }

        private static IObservable<PropertyChangedEventArgs> CreateObservable(Foo arg)
        {
            return arg.ObservePropertyChangedSlim(x => x.Value);
        }
    }
}";

            var fixedCode = @"
namespace RoslynSandbox
{
    using System;
    using System.ComponentModel;
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
            AnalyzerAssert.CodeFix<GUREA08InlineSingleLine, InlineSingleLineCodeFix>(new[] { FooCode, testCode }, fixedCode);
        }

        [Test]
        public void WhenCriteriaIsSingleLine()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value),
                ↓() => Criteria(foo))
        {
        }

        private static bool Criteria(Foo arg)
        {
            return arg.Value == 2;
        }
    }
}";

            var fixedCode = @"
namespace RoslynSandbox
{
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
            AnalyzerAssert.CodeFix<GUREA08InlineSingleLine, InlineSingleLineCodeFix>(new[] { FooCode, testCode }, fixedCode);
        }
    }
}
