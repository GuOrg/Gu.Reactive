namespace Gu.Reactive.Analyzers.Tests.GUREA02ObservableAndCriteriaMustMatchTests
{
    using Gu.Roslyn.Asserts;
    using Microsoft.CodeAnalysis.Diagnostics;
    using NUnit.Framework;

    public static class Valid
    {
        private static readonly DiagnosticAnalyzer Analyzer = new ConstructorAnalyzer();

        private const string C = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C : INotifyPropertyChanged
    {
        private int p1;
        private int p2;

        public event PropertyChangedEventHandler PropertyChanged;

        public int P1
        {
            get => this.p1;

            set
            {
                if (value == this.p1)
                {
                    return;
                }

                this.p1 = value;
                this.OnPropertyChanged();
            }
        }

        public int P2
        {
            get
            {
                return this.p2;
            }

            set
            {
                if (value == this.p2)
                {
                    return;
                }

                this.p2 = value;
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
        public static void CorrectBaseCall()
        {
            var code = @"
namespace N
{
    using Gu.Reactive;

    public class P1Condition : Condition
    {
        public P1Condition(C c)
            : base(
                c.ObservePropertyChangedSlim(x => x.P1),
                () => c.P1 == 2)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, C, code);
        }

        [Test]
        public static void IntervalAndSchedulerNow()
        {
            var code = @"
namespace N
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class P1Condition : Condition
    {
        public P1Condition(IScheduler scheduler)
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
        public static void PropertyAndSchedulerNow()
        {
            var code = @"
#pragma warning disable GUREA10 // Split up into two conditions?
namespace N
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Gu.Reactive;

    public class P1Condition : Condition
    {
        public P1Condition(C c, IScheduler scheduler)
            : base(
                Observable.Merge(
                    c.ObservePropertyChangedSlim(x => x.P1),
                    Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => (object) x)),
                () => c.P1 == 2 &&
                      scheduler.Now > DateTimeOffset.Now)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, C, code);
        }

        [Test]
        public static void CorrectNew()
        {
            var code = @"
namespace N
{
    using Gu.Reactive;

    class C1
    {
        public static ICondition Create()
        {
            var c = new C();
            return new Condition(
                c.ObservePropertyChangedSlim(x => x.P1),
                () => c.P1 == 2);
        }
    }
}";
            RoslynAssert.Valid(Analyzer, Descriptors.GUREA02ObservableAndCriteriaMustMatch, C, code);
        }

        [Test]
        public static void WhenUsingImmutableProperty()
        {
            var c1 = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class C1 : INotifyPropertyChanged
    {
        private C2 p;

        public event PropertyChangedEventHandler PropertyChanged;

        public C2 P
        {
            get => this.p;
            set
            {
                if (ReferenceEquals(value, this.p))
                {
                    return;
                }

                this.p = value;
                this.OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            var c2 = @"
namespace N
{
    public class C2
    {
        public C2(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}";
            var code = @"
namespace N
{
    using Gu.Reactive;

    public class PCondition : Condition
    {
        public PCondition(C1 c1)
            : base(
                c1.ObservePropertyChangedSlim(x => x.P),
                () => c1.P.Value == 2)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, c1, c2, code);
        }

        [Test]
        public static void WhenUsingPrivateSetPropertyOnlyAssignedInCtor()
        {
            var c1 = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class C1 : INotifyPropertyChanged
    {
        private C2 p;

        public event PropertyChangedEventHandler PropertyChanged;

        public C2 P
        {
            get => this.p;

            set
            {
                if (ReferenceEquals(value, this.p))
                {
                    return;
                }

                this.p = value;
                this.OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";

            var c2 = @"
namespace N
{
    public class C2
    {
        public C2(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }
    }
}";
            var code = @"
namespace N
{
    using Gu.Reactive;

    public class CCondition : Condition
    {
        public CCondition(C1 c1)
            : base(
                c1.ObservePropertyChangedSlim(x => x.P),
                () => c1.P.Value == 2)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, c1, c2, code);
        }

        [Test]
        public static void WhenUsingNullableValue()
        {
            var c = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class C : INotifyPropertyChanged
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

            var code = @"
namespace N
{
    using Gu.Reactive;

    public class ValueCondition : Condition
    {
        public ValueCondition(C c)
            : base(
                c.ObservePropertyChangedSlim(x => x.Value),
                () => { return c.Value is null ? (bool?)null : c.Value.Value == 1; })
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, c, code);
        }

        [Test]
        public static void IgnoreUsageInThrowOneLevel()
        {
            var c = @"
namespace N
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class C : INotifyPropertyChanged, IDisposable
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
            var code = @"
namespace N
{
    using Gu.Reactive;

    public class ValueCondition : Condition
    {
        public ValueCondition(C c)
            : base(
                c.ObservePropertyChangedSlim(x => x.Value),
                () => c.Value == 2)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, c, code);
        }

        [Test]
        public static void IgnoreUsageInThrowTwoLevels()
        {
            var c1 = @"
namespace N
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public sealed class C1 : INotifyPropertyChanged, IDisposable
    {
        private bool disposed;
        private C2 p;

        public event PropertyChangedEventHandler PropertyChanged;

        public C2 P
        {
            get
            {
                ThrowIfDisposed();
                return this.p;
            }

            set
            {
                ThrowIfDisposed();
                if (ReferenceEquals(value, this.p))
                {
                    return;
                }

                this.p = value;
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
            var c2 = @"
namespace N
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C2 : INotifyPropertyChanged, IDisposable
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
            var code = @"
namespace N
{
    using Gu.Reactive;

    public class ValueCondition : Condition
    {
        public ValueCondition(C1 c1)
            : base(
                c1.ObservePropertyChangedSlim(x => x.P.Value),
                () => c1.P?.Value == 2)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, c1, c2, code);
        }

        [Test]
        public static void IgnoreTypeofFullName()
        {
            var c = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C : INotifyPropertyChanged
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

            var code = @"
namespace N
{
    using Gu.Reactive;

    public class TextCondition : Condition
    {
        public TextCondition(C c)
            : base(
                c.ObservePropertyChanged(x => x.Text),
                () => c.Text == typeof(C).FullName)
        {
        }
    }
}";
            RoslynAssert.Valid(Analyzer, c, code);
        }

        [Test]
        public static void IgnoreRegex()
        {
            var c = @"
namespace N
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class C : INotifyPropertyChanged
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

            var code = @"
namespace N
{
    using System.Text.RegularExpressions;
    using Gu.Reactive;

    public class CCondition : Condition
    {
        public CCondition(C c)
            : base(
                c.ObservePropertyChanged(x => x.Text),
                () => Criteria(c))
        {
        }

        private static bool? Criteria(C c)
        {
            if (c.Text is string text)
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
            RoslynAssert.Valid(Analyzer, c, code);
        }
    }
}
