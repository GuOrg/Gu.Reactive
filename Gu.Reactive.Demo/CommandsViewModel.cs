namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Gu.Reactive.Demo.Annotations;
    using Gu.Wpf.Reactive;

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

            ObservingRelayCommand = new ObservingRelayCommand(() => Executed = "ObservingRelayCommand", () => CanExecute, this.ToObservable(x => x.CanExecute));
            ObservingRelayCommandWithParameter = new ObservingRelayCommand<string>(x => Executed = "ObservingRelayCommandWithParameter:" + x, x => CanExecute, this.ToObservable(x => x.CanExecute));

            var condition = new Condition(this.ToObservable(x => x.CanExecute), () => CanExecute);
            ConditionRelayCommand = new ConditionRelayCommand(() => Executed = "ObservingRelayCommand", condition);
            ConditionRelayCommandWithParameter = new ConditionRelayCommand<string>(x => Executed = "ConditionRelayCommandWithParameter: " + x, condition);
            RaiseCanExecute = new RelayCommand(
                () =>
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
                });
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
            private set
            {
                if (value.Equals(_canExecute))
                {
                    return;
                }
                _canExecute = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand RaiseCanExecute { get; private set; }
        
        public RelayCommand DelayedToggleCanExecute { get; private set; }

        public ManualRelayCommand ManualRelayCommandNoCondition { get; private set; }

        public ManualRelayCommand ManualRelayCommand { get; private set; }

        public ManualRelayCommand<string> ManualRelayCommandWithParameter { get; private set; }

        public RelayCommand RelayCommandNoCondition { get; private set; }

        public RelayCommand RelayCommand { get; private set; }

        public RelayCommand<string> RelayCommandWithParamater { get; private set; }

        public ObservingRelayCommand ObservingRelayCommand { get; private set; }
       
        public ObservingRelayCommand<string> ObservingRelayCommandWithParameter { get; private set; }

        public ConditionRelayCommand ConditionRelayCommand { get; private set; }
      
        public ConditionRelayCommand<string> ConditionRelayCommandWithParameter { get; private set; }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
