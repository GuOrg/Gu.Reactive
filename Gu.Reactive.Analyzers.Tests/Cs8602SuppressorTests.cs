namespace Gu.Reactive.Analyzers.Tests
{
    using Gu.Roslyn.Asserts;
    using NUnit.Framework;

    public static class Cs8602SuppressorTests
    {
        private static readonly Cs8602Suppressor Analyzer = new();

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

        public event PropertyChangedEventHandler? PropertyChanged;

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

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
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

        public event PropertyChangedEventHandler? PropertyChanged;

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

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
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

        [Test]
        public static void ThisObservePropertyChangedSlimTaskCompletionStatus()
        {
            var code = @"
#nullable enable
namespace Gu.Wpf.Reactive
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Reactive;

    /// <summary>
    /// A base class for running tasks and notifying about the results.
    /// </summary>
    public abstract class C : INotifyPropertyChanged
    {
        private NotifyTaskCompletion? taskCompletion;

        /// <summary>
        /// Initializes a new instance of the <see cref=""TaskRunnerBase""/> class.
        /// </summary>
        protected C()
        {
            _ = this.ObservePropertyChangedSlim(x => x.TaskCompletion.Status);
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the status of the current task.
        /// </summary>
        public NotifyTaskCompletion? TaskCompletion
        {
            get => this.taskCompletion;

            protected set
            {
                if (ReferenceEquals(value, this.taskCompletion))
                {
                    return;
                }

                this.taskCompletion = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Notifies that <paramref name=""propertyName""/> changed.
        /// </summary>
        /// <param name=""propertyName"">The name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}";
            RoslynAssert.Valid(Analyzer, code);
        }
    }
}
