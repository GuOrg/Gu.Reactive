namespace Gu.Reactive.Analyzers.Tests.GUREA03PathMustNotifyTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new InvocationAnalyzer();

        [Test]
        public static void OneLevel()
        {
            var fooCode = @"
namespace RoslynSandbox
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
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class C2
    {
        public C2()
        {
            var c1 = new C1();
            c1.ObservePropertyChanged(x => x.Value)
               .Subscribe(x => Console.WriteLine(x));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
        }

        [Test]
        public static void OneLevelGetOnly()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C1 : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Value { get; }

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

    public class C2
    {
        public C2()
        {
            var c1 = new C1();
            c1.ObservePropertyChanged(x => x.Value)
               .Subscribe(x => Console.WriteLine(x));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
        }

        [Test]
        public static void InterfaceGetSet()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public interface I : INotifyPropertyChanged
    {
        int P { get; set; }
    }
}";
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class C
    {
        public C(I i)
        {
            i.ObservePropertyChanged(x => x.P)
               .Subscribe(x => Console.WriteLine(x));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
        }

        [Test]
        public static void ObservableCollectionCount()
        {
            var code = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class C
    {
        public C(System.Collections.ObjectModel.ObservableCollection<int> xs)
        {
            xs.ObservePropertyChanged(x => x.Count)
              .Subscribe(x => Console.WriteLine(x));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, code);
        }

        [Test]
        public static void InterfaceGetOnly()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public interface I : INotifyPropertyChanged
    {
        int P { get; }
    }
}";
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class Bar
    {
        public Bar(I i)
        {
            i.ObservePropertyChanged(x => x.P)
             .Subscribe(x => Console.WriteLine(x));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
        }

        [Test]
        public static void GenericConstraint()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public interface I : INotifyPropertyChanged
    {
        int P { get; }
    }
}";
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class C<T>
        where T : class, I
    {
        public C(T t)
        {
            t.ObservePropertyChanged(x => x.P)
             .Subscribe(x => Console.WriteLine(x));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
        }

        [Test]
        public static void GenericProperty()
        {
            var iBarCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public interface IBar : INotifyPropertyChanged
    {
        int Value { get; set; }
    }
}";

            var iFooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public interface IFoo<T> : INotifyPropertyChanged
        where T: class, IBar
    {
        T Bar { get; set; }
    }
}";
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class Bar
    {
        public Bar(IFoo<IBar> foo)
        {
            foo.ObservePropertyChangedSlim(x => x.Bar.Value)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, iBarCode, iFooCode, testCode);
        }

        [Test]
        public static void GenericConstrainedProperty()
        {
            var iBarCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public interface IBar : INotifyPropertyChanged
    {
        int Value { get; set; }
    }
}";

            var iFooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public interface IFoo<T> : INotifyPropertyChanged
        where T: class, IBar
    {
        T Bar { get; set; }
    }
}";
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class Bar<T>
        where T: class, IBar
    {
        public Bar(IFoo<T> foo)
        {
            foo.ObservePropertyChangedSlim(x => x.Bar.Value)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, iBarCode, iFooCode, testCode);
        }

        [Test]
        public static void TwoLevels()
        {
            var fooCode = @"
namespace RoslynSandbox
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
namespace RoslynSandbox
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
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class Baz
    {
        public Baz()
        {
            var foo = new Foo();
            foo.ObservePropertyChanged(x => x.Bar.Value)
               .Subscribe(x => Console.WriteLine(x));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, barCode, testCode);
        }
    }
}
