namespace Gu.Reactive.Analyzers.Tests.GUREA09ObservableBeforeCriteriaTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ConstructorAnalyzer();
        private static readonly DiagnosticDescriptor Descriptor = Descriptors.GUREA09ObservableBeforeCriteria;

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
            var code = @"
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
            RoslynAssert.Valid(Analyzer, Descriptor, C1, code);
        }

        [Test]
        public static void CorrectNew()
        {
            var code = @"
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
            RoslynAssert.Valid(Analyzer, Descriptor, C1, code);
        }
    }
}
