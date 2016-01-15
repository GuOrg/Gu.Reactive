namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Gu.Wpf.Reactive;
    using JetBrains.Annotations;

    public sealed class EitherCommandsVm : INotifyPropertyChanged
    {
        private int _value;

        public EitherCommandsVm()
        {
            var isAddingOne = new Condition(this.ObservePropertyChanged(x => x.AddOneCommand.IsExecuting),
                                            () => AddOneCommand?.IsExecuting);
            var isAddingTwo = new Condition(this.ObservePropertyChanged(x => x.AddTwoCommand.IsExecuting),
                                            () => AddTwoCommand?.IsExecuting);

            var isnotAddingAny = new OrCondition(isAddingOne, isAddingTwo).Negate();
            AddOneCommand = new AsyncCommand(AddOne, isnotAddingAny);
            AddTwoCommand = new AsyncCommand(AddTwo, isnotAddingAny);
            OnPropertyChanged(nameof(AddOneCommand));
            OnPropertyChanged(nameof(AddTwoCommand));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AsyncCommand AddOneCommand { get; }

        public AsyncCommand AddTwoCommand { get; }

        public int Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task AddOne()
        {
            await Task.Delay(1000)
                      .ConfigureAwait(false);
            Value++;
        }

        private async Task AddTwo()
        {
            await Task.Delay(1000)
                      .ConfigureAwait(false);
            Value += 2;
        }
    }
}
