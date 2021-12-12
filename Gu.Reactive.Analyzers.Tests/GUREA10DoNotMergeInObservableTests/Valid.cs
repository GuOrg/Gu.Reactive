namespace Gu.Reactive.Analyzers.Tests.GUREA10DoNotMergeInObservableTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly ConstructorAnalyzer Analyzer = new();

        private const string C = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C : INotifyPropertyChanged
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
        public static void WhenNoMerge()
        {
            var code = @"
namespace N
{
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class ValueCondition : Condition
    {
        public ValueCondition(C c)
            : base(
                c.ObservePropertyChangedSlim(x => x.Value),
                () => c.Value == 2)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, C, code);
        }
    }
}
