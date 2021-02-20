namespace Gu.Reactive.Demo.UiTestWindows
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Wpf.Reactive;

    public sealed class ConditionControlViewModel : IDisposable
    {
        private bool disposed;

        public ConditionControlViewModel()
        {
            this.Condition = new OrCondition(
                new DynamicCondition(this.Values.AsReadOnlyFilteredView(x => true), x => new IsEvenGretarThanTwo(x)),
                new DynamicCondition(this.Values.AsReadOnlyFilteredView(x => true), x => new IsLessThanFive(x)));
            this.ClearCommand = new RelayCommand(() => this.Values.Clear());
        }

        public ObservableCollection<WithInt> Values { get; } = new ObservableCollection<WithInt>
        {
            new WithInt { Value = 1 },
            new WithInt { Value = 2 },
            new WithInt { Value = 3 },
            new WithInt { Value = 4 },
        };

        public RelayCommand ClearCommand { get; }

        public ICondition Condition { get; }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Condition.Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(ConditionControlViewModel));
            }
        }

        public class WithInt : INotifyPropertyChanged
        {
            private int value;

            public event PropertyChangedEventHandler? PropertyChanged;

            public int Value
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

            protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private sealed class IsEvenGretarThanTwo : AndCondition
        {
            internal IsEvenGretarThanTwo(WithInt x)
                : base(new IsEven(x), new IsGreaterThanTwo(x))
            {
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    foreach (var prerequisite in this.Prerequisites)
                    {
                        prerequisite.Dispose();
                    }
                }

                base.Dispose(disposing);
            }
        }

        private sealed class IsEven : Condition
        {
            internal IsEven(WithInt withInt)
                : base(
                    withInt.ObservePropertyChangedSlim(x => x.Value),
                    () => withInt.Value % 2 == 0)
            {
            }
        }

        private sealed class IsGreaterThanTwo : Condition
        {
            internal IsGreaterThanTwo(WithInt withInt)
                : base(
                    withInt.ObservePropertyChangedSlim(x => x.Value),
                    () => withInt.Value > 2)
            {
            }
        }

        private sealed class IsLessThanFive : Condition
        {
            internal IsLessThanFive(WithInt withInt)
                : base(
                    withInt.ObservePropertyChangedSlim(x => x.Value),
                    () => withInt.Value < 5)
            {
            }
        }

        private class DynamicCondition : OrCondition
        {
            internal DynamicCondition(IReadOnlyView<WithInt> xs, Func<WithInt, ICondition> map)
                  : base(
                      xs.AsMappingView(map, onRemove: x => x.Dispose()),
                      leaveOpen: false)
            {
            }
        }
    }
}
