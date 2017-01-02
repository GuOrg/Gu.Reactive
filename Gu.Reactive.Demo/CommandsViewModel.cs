namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using JetBrains.Annotations;
    using Wpf.Reactive;

    public class CommandsViewModel : INotifyPropertyChanged
    {
        private string executed;

        private bool canExecute;

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

            var condition = new Condition(this.ObservePropertyChanged(x => x.CanExecute), () => this.CanExecute);
            this.ConditionRelayCommand = new ConditionRelayCommand(() => this.Executed = "ObservingRelayCommand", condition);
            this.ConditionRelayCommandWithParameter = new ConditionRelayCommand<string>(x => this.Executed = "ConditionRelayCommandWithParameter: " + x, condition);
            this.RaiseCanExecuteCommand = new RelayCommand(this.RaiseCanExecute);
            this.RaiseCanExecuteOnOtherThread = new RelayCommand(() => Task.Run(() => this.RaiseCanExecute()));
            this.DelayedToggleCanExecute = new RelayCommand(async () =>
                    {
                        await Task.Delay(500).ConfigureAwait(false);
                        this.CanExecute = !this.CanExecute;
                    });
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public RelayCommand RaiseCanExecuteCommand { get; private set; }

        public RelayCommand RaiseCanExecuteOnOtherThread { get; private set; }

        public RelayCommand DelayedToggleCanExecute { get; private set; }

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
