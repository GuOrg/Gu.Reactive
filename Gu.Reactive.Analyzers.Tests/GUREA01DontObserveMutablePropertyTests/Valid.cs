namespace Gu.Reactive.Analyzers.Tests.GUREA01DontObserveMutablePropertyTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();

        [Test]
        public static void ObservingLocal()
        {
            var fooCode = @"
namespace RoslynSandbox
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
            var code = @"namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    internal class C2
    {
        public C2()
        {
            var c1 = new C1();
            c1.ObservePropertyChangedSlim(x => x.Value)
              .Subscribe(_ => Console.WriteLine(""c""));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, code);
        }

        [Test]
        public static void ObservingGetOnlyPropertyInSelf()
        {
            var fooCode = @"
namespace RoslynSandbox
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
            var testCode = @"namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    internal class C2
    {
        public C2()
        {
            this.C1 = new C1();
            this.C1.ObservePropertyChanged(x => x.Value)
                   .Subscribe(x => Console.WriteLine(x));
        }

        public C1 C1 { get; }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
        }
    }
}
