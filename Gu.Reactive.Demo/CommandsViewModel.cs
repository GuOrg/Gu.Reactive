namespace Gu.Reactive.Demo
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Wpf.Reactive;

    public sealed class CommandsViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly Condition condition;

        private string executed;
        private bool canExecute;
        private bool disposed;

        public CommandsViewModel()
        {
            this.ManualRelayCommandNoCondition = new ManualRelayCommand(() => this.Executed = "ManualRelayCommandNoCondition");
            this.ManualRelayCommand = new ManualRelayCommand(() => this.Executed = "ManualRelayCommand", () => this.CanExecute);
            this.ManualRelayCommandWithParameter = new ManualRelayCommand<string>(x => this.Executed = "ManualRelayCommandWithParameter: " + x, _ => this.CanExecute);

            this.RelayCommandNoCondition = new RelayCommand(() => this.Executed = "RelayCommandNoCondition");
            this.RelayCommand = new RelayCommand(() => this.Executed = "RelayCommand", () => this.CanExecute);
            this.RelayCommandWithParamater = new RelayCommand<string>(x => this.Executed = "RelayCommandWithParamater: " + x, x => this.CanExecute);

            this.ObservingRelayCommand = new ObservingRelayCommand(() => this.Executed = "ObservingRelayCommand", () => this.CanExecute, this.ObservePropertyChanged(x => x.CanExecute));
            this.ObservingRelayCommandWithParameter = new ObservingRelayCommand<string>(x => this.Executed = "ObservingRelayCommandWithParameter:" + x, x => this.CanExecute, this.ObservePropertyChanged(x => x.CanExecute));

            this.condition = new Condition(this.ObservePropertyChanged(x => x.CanExecute), () => this.CanExecute);
            this.ConditionRelayCommand = new ConditionRelayCommand(() => this.Executed = "ObservingRelayCommand", this.condition);
            this.ConditionRelayCommandWithParameter = new ConditionRelayCommand<string>(x => this.Executed = "ConditionRelayCommandWithParameter: " + x, this.condition);
            this.RaiseCanExecuteCommand = new RelayCommand(this.RaiseCanExecute);
            this.RaiseCanExecuteOnOtherThread = new RelayCommand(() => Task.Run(() => this.RaiseCanExecute()));
            this.DelayedToggleCanExecute = new RelayCommand(async () =>
                    {
                        await Task.Delay(500).ConfigureAwait(false);
                        this.CanExecute = !this.CanExecute;
                    });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand RaiseCanExecuteCommand { get; }

        public RelayCommand RaiseCanExecuteOnOtherThread { get; }

        public RelayCommand DelayedToggleCanExecute { get; }

        public ManualRelayCommand ManualRelayCommandNoCondition { get; }

        public ManualRelayCommand ManualRelayCommand { get; }

        public ManualRelayCommand<string> ManualRelayCommandWithParameter { get; }

        public RelayCommand RelayCommandNoCondition { get; }

        public RelayCommand RelayCommand { get; }

        public RelayCommand<string> RelayCommandWithParamater { get; }

        public ObservingRelayCommand ObservingRelayCommand { get; }

        public ObservingRelayCommand<string> ObservingRelayCommandWithParameter { get; }

        public ConditionRelayCommand ConditionRelayCommand { get; }

        public ConditionRelayCommand<string> ConditionRelayCommandWithParameter { get; }

        public string Executed
        {
            get
            {
                return this.executed;
            }

            private set
            {
                if (value == this.executed)
                {
                    return;
                }

                this.executed = value;
                this.OnPropertyChanged();
            }
        }

        public bool CanExecute
        {
            get
            {
                return this.canExecute;
            }

            set
            {
                if (value.Equals(this.canExecute))
                {
                    return;
                }

                this.canExecute = value;
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
            this.ObservingRelayCommand.Dispose();
            this.ObservingRelayCommandWithParameter.Dispose();
            this.ConditionRelayCommand.Dispose();
            this.ConditionRelayCommandWithParameter.Dispose();
            this.condition.Dispose();
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RaiseCanExecute()
        {
            this.ManualRelayCommandNoCondition.RaiseCanExecuteChanged();
            this.ManualRelayCommand.RaiseCanExecuteChanged();
            this.ManualRelayCommandWithParameter.RaiseCanExecuteChanged();

            this.RelayCommandNoCondition.RaiseCanExecuteChanged();
            this.RelayCommand.RaiseCanExecuteChanged();
            this.RelayCommandWithParamater.RaiseCanExecuteChanged();

            this.ObservingRelayCommand.RaiseCanExecuteChanged();
            this.ObservingRelayCommandWithParameter.RaiseCanExecuteChanged();

            this.ConditionRelayCommand.RaiseCanExecuteChanged();
            this.ConditionRelayCommandWithParameter.RaiseCanExecuteChanged();
        }
    }
}
