namespace Gu.Reactive.Analyzers.Tests.GUREA02ObservableAndCriteriaMustMatchTests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    internal class HappyPath
    {
        private const string FooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Foo : INotifyPropertyChanged
    {
        private int value1;
        private int value2;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value1
        {
            get
            {
                return this.value1;
            }

            set
            {
                if (value == this.value1)
                {
                    return;
                }

                this.value1 = value;
                this.OnPropertyChanged();
            }
        }

        public int Value2
        {
            get
            {
                return this.value2;
            }

            set
            {
                if (value == this.value2)
                {
                    return;
                }

                this.value2 = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

        [Test]
        public void Correct()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value1),
                () => foo.Value1 == 2)
        {
        }
    }
}";
            AnalyzerAssert.Valid<GUREA02ObservableAndCriteriaMustMatch>(FooCode, testCode);
        }

        [Test]
        public void WhenUsingImmutableProperty()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class Foo : INotifyPropertyChanged
    {
        private Bar bar;

        public event PropertyChangedEventHandler PropertyChanged;

        public Bar Bar
        {
            get => this.bar;

            set
            {
                if (ReferenceEquals(value, this.bar))
                {
                    return;
                }

                this.bar = value;
                this.OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            var barCode = @"
namespace RoslynSandbox
{
    public class Bar
    {
        public Bar(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}";
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Bar),
                () => foo.Bar.Value == 2)
        {
        }
    }
}";
            AnalyzerAssert.Valid<GUREA02ObservableAndCriteriaMustMatch>(fooCode, barCode, testCode);
        }

        [Test]
        public void WhenUsingNullableValue()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class Foo : INotifyPropertyChanged
    {
        private int? value;

        public event PropertyChangedEventHandler PropertyChanged;

        public int? Value
        {
            get => this.value;

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

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value),
                () => { return foo.Value == null ? (bool?)null : foo.Value.Value == 1; })
        {
        }
    }
}";
            AnalyzerAssert.Valid<GUREA02ObservableAndCriteriaMustMatch>(fooCode, testCode);
        }

        [Test]
        public void IgnoreUsageInThrowOneLevel()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class Foo : INotifyPropertyChanged, IDisposable
    {
        private int value;
        private bool disposed;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get
            {
                ThrowIfDisposed();
                return this.value;
            }

            set
            {
                ThrowIfDisposed();
                if (value == this.value)
                {
                    return;
                }

                this.value = value;
                this.OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}";
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value),
                () => foo.Value == 2)
        {
        }
    }
}";
            AnalyzerAssert.Valid<GUREA02ObservableAndCriteriaMustMatch>(fooCode, testCode);
        }

        [Test]
        public void IgnoreUsageInThrowTwoLevels()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class Foo : INotifyPropertyChanged, IDisposable
    {
        private bool disposed;
        private Bar bar;

        public event PropertyChangedEventHandler PropertyChanged;

        public Bar Bar
        {
            get
            {
                ThrowIfDisposed();
                return this.bar;
            }

            set
            {
                ThrowIfDisposed();
                if (ReferenceEquals(value, this.bar))
                {
                    return;
                }

                this.bar = value;
                this.OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}";
            var barCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Bar : INotifyPropertyChanged, IDisposable
    {
        private int value;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get
            {
                ThrowIfDisposed();
                return this.value;
            }

            set
            {
                ThrowIfDisposed();
                if (value == this.value)
                {
                    return;
                }

                this.value = value;
                this.OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}";
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChangedSlim(x => x.Value),
                () => foo.Value == 2)
        {
        }
    }
}";
            AnalyzerAssert.Valid<GUREA02ObservableAndCriteriaMustMatch>(fooCode, barCode, testCode);
        }
    }
}