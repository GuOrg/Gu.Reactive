namespace Gu.Reactive.Analyzers.Tests.GUREA03PathMustNotifyTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Diagnostics
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();
        private static readonly ExpectedDiagnostic ExpectedDiagnostic = ExpectedDiagnostic.Create(Descriptors.GUREA03PathMustNotify);

        [Test]
        public static void OneLevel()
        {
            var c1 = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C1 : INotifyPropertyChanged
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
            var code = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class C2
    {
        public C2()
        {
            var c1 = new C1();
            c1.ObservePropertyChanged(x => x.↓Value)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, c1, code);
        }

        [Test]
        public static void OneLevelMethod()
        {
            var c1 = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C1 : INotifyPropertyChanged
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
            var code = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class C2
    {
        public C2()
        {
            var c1 = new C1();
            c1.ObservePropertyChanged(↓x => x.ToString())
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, c1, code);
        }

        [Test]
        public static void TwoLevelsFirstNotNotifying()
        {
            var c1 = @"
namespace N
{
    using System.ComponentModel;

    public class C1 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public C2 P { get; set; }
    }
}";
            var c2 = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C2 : INotifyPropertyChanged
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
            var code = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class C3
    {
        public C3()
        {
            var c1 = new C1();
            c1.ObservePropertyChanged(x => x.↓P.Value)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, c1, c2, code);
        }

        [Test]
        public static void TwoLevelsLastNotNotifying()
        {
            var c1 = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C1 : INotifyPropertyChanged
    {
        private C2 p;

        public event PropertyChangedEventHandler PropertyChanged;

        public C2 P
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
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C2 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Value { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";
            var code = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class C3
    {
        public C3()
        {
            var c1 = new C1();
            c1.ObservePropertyChanged(x => x.P.↓Value)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.Diagnostics(Analyzer, ExpectedDiagnostic, c1, c2, code);
        }
    }
}
