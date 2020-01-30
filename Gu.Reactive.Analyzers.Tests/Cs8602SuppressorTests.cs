namespace Gu.Reactive.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Cs8602SuppressorTests
    {
        private static readonly DiagnosticAnalyzer Analyzer = new Cs8602Suppressor();

        [TestCase("ObservePropertyChanged(x => x.P.P)")]
        [TestCase("ObservePropertyChangedSlim(x => x.P.P)")]
        [TestCase("ObserveFullPropertyPathSlim(x => x.P.P)")]
        [TestCase("ObserveValue(x => x.P.P)")]
        public static void TwoLevels(string expression)
        {
            var c1 = @"
#nullable enable
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C1 : INotifyPropertyChanged
    {
        private C2? p;

        public event PropertyChangedEventHandler PropertyChanged;

        public C2? P
        {
            get => this.p;
            set
            {
                if (value == this.p)
                {
                    return;
                }

                this.p = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";
            var c2 = @"
#nullable enable
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C2 : INotifyPropertyChanged
    {
        private int p;

        public event PropertyChangedEventHandler PropertyChanged;

        public int P
        {
            get => this.p;
            set
            {
                if (value == this.p)
                {
                    return;
                }

                this.p = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";
            var code = @"
#nullable enable
namespace N
{
    using System;
    using Gu.Reactive;

    public class C
    {
        public C()
        {
            var c1 = new C1();
            c1.ObservePropertyChanged(x => x.P.P)
               .Subscribe(x => Console.WriteLine(x));
        }
    }
}".AssertReplace("ObservePropertyChanged(x => x.P.P)", expression);
            RoslynAssert.Valid(Analyzer, c1, c2, code);
        }
    }
}
