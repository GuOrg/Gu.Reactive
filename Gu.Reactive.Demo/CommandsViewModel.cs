namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Annotations;
    using Wpf.Reactive;

    public class CommandsViewModel : INotifyPropertyChanged
    {
        private string _executed;

        private bool _canExecute;

        public CommandsViewModel()
        {
            ManualRelayCommandNoCondition = new ManualRelayCommand(() => Executed = "ManualRelayCommandNoCondition");
            ManualRelayCommand = new ManualRelayCommand(() => Executed = "ManualRelayCommand", () => CanExecute);
            ManualRelayCommandWithParameter = new ManualRelayCommand<string>(x => Executed = "ManualRelayCommandWithParameter: " + x, _ => CanExecute);

            RelayCommandNoCondition = new RelayCommand(() => Executed = "RelayCommandNoCondition");
            RelayCommand = new RelayCommand(() => Executed = "RelayCommand", () => CanExecute);
            RelayCommandWithParamater = new RelayCommand<string>(x => Executed = "RelayCommandWithParamater: " + x, x => CanExecute);

            ObservingRelayCommand = new ObservingRelayCommand(() => Executed = "ObservingRelayCommand", () => CanExecute, this.ObservePropertyChanged(x => x.CanExecute));
            ObservingRelayCommandWithParameter = new ObservingRelayCommand<string>(x => Executed = "ObservingRelayCommandWithParameter:" + x, x => CanExecute, this.ObservePropertyChanged(x => x.CanExecute));

            var condition = new Condition(this.ObservePropertyChanged(x => x.CanExecute), () => CanExecute);
            ConditionRelayCommand = new ConditionRelayCommand(() => Executed = "ObservingRelayCommand", condition);
            ConditionRelayCommandWithParameter = new ConditionRelayCommand<string>(x => Executed = "ConditionRelayCommandWithParameter: " + x, condition);
            RaiseCanExecuteCommand = new RelayCommand(RaiseCanExecute);
            RaiseCanExecuteOnOtherThread = new RelayCommand(() => Task.Run(() => RaiseCanExecute()));
            DelayedToggleCanExecute = new RelayCommand(async () =>
                    {
                        await Task.Delay(500);
                        CanExecute = !CanExecute;
                    });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Executed
        {
            get
            {
                return _executed;
            }
            private set
            {
                if (value == _executed)
                {
                    return;
                }
                _executed = value;
                OnPropertyChanged();
            }
        }

        public bool CanExecute
        {
            get
            {
                return _canExecute;
            }
            set
            {
                if (value.Equals(_canExecute))
                {
                    return;
                }
                _canExecute = value;
                OnPropertyChanged();
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
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void RaiseCanExecute()
        {
            ManualRelayCommandNoCondition.RaiseCanExecuteChanged();
            ManualRelayCommand.RaiseCanExecuteChanged();
            ManualRelayCommandWithParameter.RaiseCanExecuteChanged();

            RelayCommandNoCondition.RaiseCanExecuteChanged();
            RelayCommand.RaiseCanExecuteChanged();
            RelayCommandWithParamater.RaiseCanExecuteChanged();

            ObservingRelayCommand.RaiseCanExecuteChanged();
            ObservingRelayCommandWithParameter.RaiseCanExecuteChanged();

            ConditionRelayCommand.RaiseCanExecuteChanged();
            ConditionRelayCommandWithParameter.RaiseCanExecuteChanged();
        }

    }
}
