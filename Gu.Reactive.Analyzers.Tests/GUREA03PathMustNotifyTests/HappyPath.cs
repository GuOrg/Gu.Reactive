namespace Gu.Reactive.Analyzers.Tests.GUREA03PathMustNotifyTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal class HappyPath
    {
        static HappyPath()
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

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            foo.ObservePropertyChanged(x => x.Value)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            AnalyzerAssert.NoDiagnostics<GUREA03PathMustNotify>(fooCode, testCode);
        }

        [Test]
        public void OneLevelGetOnly()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Foo : INotifyPropertyChanged
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

    public class Bar
    {
        public Bar()
        {
            var foo = new Foo();
            foo.ObservePropertyChanged(x => x.Value)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            AnalyzerAssert.NoDiagnostics<GUREA03PathMustNotify>(fooCode, testCode);
        }

        [Test]
        public void InterfaceGetSet()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public interface IFoo : INotifyPropertyChanged
    {
        public int Value { get; set; }
    }
}";
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class Bar
    {
        public Bar(IFoo foo)
        {
            foo.ObservePropertyChanged(x => x.Value)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            AnalyzerAssert.NoDiagnostics<GUREA03PathMustNotify>(fooCode, testCode);
        }

        [Test]
        public void ObservableCollectionCount()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class Bar
    {
        public Bar(System.Collections.ObjectModel.ObservableCollection<int> foo)
        {
            foo.ObservePropertyChanged(x => x.Count)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            AnalyzerAssert.NoDiagnostics<GUREA03PathMustNotify>(testCode);
        }

        [Test]
        public void InterfaceGetOnly()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public interface IFoo : INotifyPropertyChanged
    {
        public int Value { get; }
    }
}";
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class Bar
    {
        public Bar(IFoo foo)
        {
            foo.ObservePropertyChanged(x => x.Value)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            AnalyzerAssert.NoDiagnostics<GUREA03PathMustNotify>(fooCode, testCode);
        }

        [Test]
        public void GenericConstraint()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public interface IFoo : INotifyPropertyChanged
    {
        public int Value { get; }
    }
}";
            var testCode = @"
namespace RoslynSandbox
{
    using System;
    using Gu.Reactive;

    public class Bar<T>
        where T : class, IFoo
    {
        public Bar(T foo)
        {
            foo.ObservePropertyChanged(x => x.Value)
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            AnalyzerAssert.NoDiagnostics<GUREA03PathMustNotify>(fooCode, testCode);
        }

        [Test]
        public void TwoLevels()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Foo : INotifyPropertyChanged
    {
        private Bar bar;

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
    }
";
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
               .Subscribe(_ => Console.WriteLine(string.Empty));
        }
    }
}";
            AnalyzerAssert.NoDiagnostics<GUREA03PathMustNotify>(fooCode, barCode, testCode);
        }
    }
}