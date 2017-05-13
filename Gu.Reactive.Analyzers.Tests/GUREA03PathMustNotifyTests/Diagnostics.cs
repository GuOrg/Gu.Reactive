namespace Gu.Reactive.Analyzers.Tests.GUREA03PathMustNotifyTests
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
        public void OneLevel()
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

        public int Value { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        public Bar()
        {
            var foo = new Foo();
            foo.ObservePropertyChanged(x => x.↓Value)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            AnalyzerAssert.Diagnostics<GUREA03PathMustNotify>(fooCode, testCode);
        }
    }
}
