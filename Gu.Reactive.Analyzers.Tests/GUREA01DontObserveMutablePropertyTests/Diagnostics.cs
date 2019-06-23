namespace Gu.Reactive.Analyzers.Tests.GUREA01DontObserveMutablePropertyTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class Diagnostics
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(GUREA01DontObserveMutableProperty.Descriptor);

        [Test]
        public void ObservingMutablePropertyInSelf()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    internal class Foo : INotifyPropertyChanged
    {
        private int _value;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (value == _value)
                {
                    return;
                }

                _value = value;
                OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";
            var testCode = @"namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    internal class Meh
    {
        public Meh()
        {
            this.Foo = new Foo();
            this.Foo.↓ObserveFullPropertyPathSlim(x => x.Value)
                    .Subscribe(_ => Console.WriteLine(""meh""));
        }

        public Foo Foo { get; set; }
    }
}";
            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, fooCode, testCode);
        }
    }
}
