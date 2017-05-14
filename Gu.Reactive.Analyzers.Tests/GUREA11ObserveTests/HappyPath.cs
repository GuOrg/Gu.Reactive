namespace Gu.Reactive.Analyzers.Tests.GUREA11ObserveTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal class HappyPath
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
        static HappyPath()
        {
            AnalyzerAssert.MetadataReference.AddRange(MetadataReferences.All);
        }

        [Test]
        public void WhenNoMerge()
        {
            var testCode = @"
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
            AnalyzerAssert.NoDiagnostics<GUREA11Observe>(FooCode, testCode);
        }
    }
}