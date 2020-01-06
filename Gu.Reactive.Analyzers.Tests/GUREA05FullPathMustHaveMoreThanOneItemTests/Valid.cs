namespace Gu.Reactive.Analyzers.Tests.GUREA05FullPathMustHaveMoreThanOneItemTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();

        [Test]
        public static void TwoLevels()
        {
            var fooCode = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Foo : INotifyPropertyChanged
    {
        private Bar bar;

        public event PropertyChangedEventHandler PropertyChanged;

        public Bar Bar
        {
            get
            {
                return this.bar;
            }

            set
            {
                if (value == this.bar)
                {
                    return;
                }

                this.bar = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";
            var barCode = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Bar : INotifyPropertyChanged
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
            var testCode = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class Baz
    {
        public Baz()
        {
            var foo = new Foo();
            foo.ObserveFullPropertyPathSlim(x => x.Bar.Value)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, barCode, testCode);
        }
    }
}
