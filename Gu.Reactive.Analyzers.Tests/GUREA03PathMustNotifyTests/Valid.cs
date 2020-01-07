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
            var c1 = @"
namespace N
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
            var c2 = @"
namespace N
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
            RoslynAssert.Valid(Analyzer, c1, c2);
        }

        [Test]
        public static void OneLevelGetOnly()
        {
            var c1 = @"
namespace N
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
            var c2 = @"
namespace N
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
            RoslynAssert.Valid(Analyzer, c1, c2);
        }

        [Test]
        public static void InterfaceGetSet()
        {
            var i = @"
namespace N
{
    using System.ComponentModel;

    public interface I : INotifyPropertyChanged
    {
        int P { get; set; }
    }
}";
            var testCode = @"
namespace N
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
            RoslynAssert.Valid(Analyzer, i, testCode);
        }

        [Test]
        public static void ObservableCollectionCount()
        {
            var code = @"
namespace N
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
            var i = @"
namespace N
{
    using System.ComponentModel;

    public interface I : INotifyPropertyChanged
    {
        int P { get; }
    }
}";
            var code = @"
namespace N
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
            RoslynAssert.Valid(Analyzer, i, code);
        }

        [Test]
        public static void GenericConstraint()
        {
            var i = @"
namespace N
{
    using System.ComponentModel;

    public interface I : INotifyPropertyChanged
    {
        int P { get; }
    }
}";
            var code = @"
namespace N
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
            RoslynAssert.Valid(Analyzer, i, code);
        }

        [Test]
        public static void GenericProperty()
        {
            var i = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public interface I : INotifyPropertyChanged
    {
        int Value { get; set; }
    }
}";

            var iOfT = @"
namespace N
{
    using System.ComponentModel;

    public interface I<T> : INotifyPropertyChanged
        where T: class, I
    {
        T P { get; set; }
    }
}";
            var code = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class C
    {
        public C(I<I> i)
        {
            i.ObservePropertyChangedSlim(x => x.P.Value)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, i, iOfT, code);
        }

        [Test]
        public static void GenericConstrainedProperty()
        {
            var i = @"
namespace N
{
    using System.ComponentModel;

    public interface I : INotifyPropertyChanged
    {
        int Value { get; set; }
    }
}";

            var iOfT = @"
namespace N
{
    using System.ComponentModel;

    public interface I<T> : INotifyPropertyChanged
        where T: class, I
    {
        T P { get; set; }
    }
}";
            var cOfT = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class C<T>
        where T: class, I
    {
        public C(I<T> i)
        {
            i.ObservePropertyChangedSlim(x => x.P.Value)
             .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, i, iOfT, cOfT);
        }

        [Test]
        public static void TwoLevels()
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
            get
            {
                return this.p;
            }

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
            var c3 = @"
namespace N
{
    using System;
    using Gu.Reactive;

    public class C3
    {
        public C3()
        {
            var c1 = new C1();
            c1.ObservePropertyChanged(x => x.P.Value)
               .Subscribe(x => Console.WriteLine(x));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, c1, c2, c3);
        }
    }
}
