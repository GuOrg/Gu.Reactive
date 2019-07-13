namespace Gu.Reactive.Analyzers.Tests.GUREA02ObservableAndCriteriaMustMatchTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public class ValidCode
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ConstructorAnalyzer();

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
            get => this.value1;

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
        public void CorrectBaseCall()
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
            RoslynAssert.Valid(Analyzer, FooCode, testCode);
        }

        [Test]
        public void IntervalAndSchedulerNow()
        {
            var code = @"
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(IScheduler scheduler)
            : base(
                Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => (object) x),
                () => scheduler.Now > DateTimeOffset.Now)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, code);
        }

        [Test]
        public void PropertyAndSchedulerNow()
        {
            var testCode = @"
#pragma warning disable GUREA10 // Split up into two conditions?
namespace RoslynSandbox
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo, IScheduler scheduler)
            : base(
                Observable.Merge(
                    foo.ObservePropertyChangedSlim(x => x.Value1),
                    Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => (object) x)),
                () => foo.Value1 == 2 &&
                      scheduler.Now > DateTimeOffset.Now)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, FooCode, testCode);
        }

        [Test]
        public void CorrectNew()
        {
            var testCode = @"
namespace RoslynSandbox
{
    using Gu.Reactive;

    class Bar
    {
        public static ICondition Create()
        {
            var foo = new Foo();
            return new Condition(
                foo.ObservePropertyChangedSlim(x => x.Value1),
                () => foo.Value1 == 2);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, GUREA02ObservableAndCriteriaMustMatch.Descriptor, FooCode, testCode);
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
            RoslynAssert.Valid(Analyzer, fooCode, barCode, testCode);
        }

        [Test]
        public void WhenUsingPrivateSetPropertyOnlyAssignedInCtor()
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

        public int Value { get; private set; }
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
            RoslynAssert.Valid(Analyzer, fooCode, barCode, testCode);
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
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
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
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
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
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Bar : INotifyPropertyChanged, IDisposable
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
                foo.ObservePropertyChangedSlim(x => x.Bar.Value),
                () => foo.Bar?.Value == 2)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, barCode, testCode);
        }

        [Test]
        public void IgnoreTypeofFullName()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Foo : INotifyPropertyChanged
    {
        private string text;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Text
        {
            get => this.text;
            set
            {
                if (value == this.text)
                {
                    return;
                }

                this.text = value;
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
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChanged(x => x.Text),
                () => foo.Text == typeof(Foo).FullName)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
        }

        [Test]
        public void IgnoreRegex()
        {
            var fooCode = @"
namespace RoslynSandbox
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Foo : INotifyPropertyChanged
    {
        private string text;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Text
        {
            get => this.text;
            set
            {
                if (value == this.text)
                {
                    return;
                }

                this.text = value;
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
    using System.Text.RegularExpressions;
    using Gu.Reactive;

    public class FooCondition : Condition
    {
        public FooCondition(Foo foo)
            : base(
                foo.ObservePropertyChanged(x => x.Text),
                () => Criteria(foo))
        {
        }

        private static bool? Criteria(Foo foo)
        {
            if (foo.Text is string text)
            {
                var match = Regex.Match(text, string.Empty);
                return match.Success &&
                       int.TryParse(match.Groups[string.Empty].Value, out var value) &&
                       value > 10;
            }

            return false;
        }
    }
}";
            RoslynAssert.Valid(Analyzer, fooCode, testCode);
        }
    }
}
