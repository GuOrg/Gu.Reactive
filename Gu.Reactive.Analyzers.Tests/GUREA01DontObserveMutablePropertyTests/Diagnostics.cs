namespace Gu.Reactive.Analyzers.Tests.GUREA01DontObserveMutablePropertyTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA01DoNotObserveMutableProperty);

        [Test]
        public static void ObservingMutablePropertyInSelf()
        {
            var c1 = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    internal class C1 : INotifyPropertyChanged
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
            var code = @"
namespace N
{
    using System;
    using Gu.Reactive;

    internal class C2
    {
        public C2()
        {
            this.C1 = new C1();
            this.C1.↓ObserveFullPropertyPathSlim(x => x.Value)
                   .Subscribe(_ => Console.WriteLine(""c2""));
        }

        public C1 C1 { get; set; }
    }
}";
            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, c1, code);
        }
    }
}
