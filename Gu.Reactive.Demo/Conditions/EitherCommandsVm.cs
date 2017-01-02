namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Gu.Wpf.Reactive;
    using JetBrains.Annotations;

    public sealed class EitherCommandsVm : INotifyPropertyChanged
    {
        private int value;

        public EitherCommandsVm()
        {
            var isAddingOne = new Condition(this.ObservePropertyChanged(x => x.AddOneCommand.IsExecuting),
                                            () => this.AddOneCommand?.IsExecuting);
            var isAddingTwo = new Condition(this.ObservePropertyChanged(x => x.AddTwoCommand.IsExecuting),
                                            () => this.AddTwoCommand?.IsExecuting);

            var isnotAddingAny = new OrCondition(isAddingOne, isAddingTwo).Negate();
            this.AddOneCommand = new AsyncCommand(this.AddOne, isnotAddingAny);
            this.AddTwoCommand = new AsyncCommand(this.AddTwo, isnotAddingAny);
            this.OnPropertyChanged(nameof(this.AddOneCommand));
            this.OnPropertyChanged(nameof(this.AddTwoCommand));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncCommand AddOneCommand { get; }

        public AsyncCommand AddTwoCommand { get; }

        public int Value
        {
            get { return this.value; }

            set
            {
                if (value == this.value) return;
                this.value = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task AddOne()
        {
            await Task.Delay(1000)
                      .ConfigureAwait(false);
            this.Value++;
        }

        private async Task AddTwo()
        {
            await Task.Delay(1000)
                      .ConfigureAwait(false);
            this.Value += 2;
        }
    }
}
