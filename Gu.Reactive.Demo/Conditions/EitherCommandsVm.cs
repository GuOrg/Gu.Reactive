namespace Gu.Reactive.Demo
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Gu.Wpf.Reactive;

    public sealed class EitherCommandsVm : INotifyPropertyChanged, IDisposable
    {
        private readonly Condition isAddingOne;
        private readonly Condition isAddingTwo;
        private readonly ICondition isNotAddingAny;

        private int value;
        private bool disposed;

        public EitherCommandsVm()
        {
            this.isAddingOne = new Condition(
                this.ObservePropertyChangedSlim(x => x.AddOneCommand.IsExecuting),
                () => this.AddOneCommand?.IsExecuting);
            this.isAddingTwo = new Condition(
                this.ObservePropertyChangedSlim(x => x.AddTwoCommand.IsExecuting),
                () => this.AddTwoCommand?.IsExecuting);

#pragma warning disable IDISP004 // Don't ignore created IDisposable.
            this.isNotAddingAny = new OrCondition(this.isAddingOne, this.isAddingTwo).Negate();
#pragma warning restore IDISP004 // Don't ignore created IDisposable.
            this.AddOneCommand = new AsyncCommand(this.AddOneAsync, this.isNotAddingAny);
            this.AddTwoCommand = new AsyncCommand(this.AddTwoAsync, this.isNotAddingAny);
            this.OnPropertyChanged(nameof(this.AddOneCommand));
            this.OnPropertyChanged(nameof(this.AddTwoCommand));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public AsyncCommand AddOneCommand { get; }

        public AsyncCommand AddTwoCommand { get; }

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

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.AddOneCommand.Dispose();
            this.AddTwoCommand.Dispose();
            this.isAddingOne.Dispose();
            this.isAddingTwo.Dispose();
            this.isNotAddingAny.Dispose();
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task AddOneAsync()
        {
            await Task.Delay(1000)
                      .ConfigureAwait(false);
            this.Value++;
        }

        private async Task AddTwoAsync()
        {
            await Task.Delay(1000)
                      .ConfigureAwait(false);
            this.Value += 2;
        }
    }
}
