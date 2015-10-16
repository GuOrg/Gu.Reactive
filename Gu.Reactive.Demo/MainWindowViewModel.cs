namespace Gu.Reactive.Demo
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using Annotations;
    using Wpf.Reactive;

    using Condition = Condition;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly List<ICondition> _conditions;
        private bool _isDoorClosed;
        private bool _isMotorRunning;

        public MainWindowViewModel()
        {
            IsDoorClosedCondition = new Condition(
                this.ObservePropertyChanged(x => x.IsDoorClosed), 
                () => IsDoorClosed) { Name = "Door open" };

            IsMotorRunningCondition = new Condition(
                this.ObservePropertyChanged(x => x.IsMotorRunning), 
                () => IsMotorRunning) { Name = "Motor running" };

            AndCondition = new AndCondition(IsDoorClosedCondition, IsMotorRunningCondition)
            {
                Name = "Can start"
            };

            NegatedCondition = IsDoorClosedCondition.Negate();

            _conditions = new List<ICondition>
            {
                IsDoorClosedCondition, 
                IsMotorRunningCondition, 
                AndCondition, 
                NegatedCondition
            };

            StartCommand = new ConditionRelayCommand<string>(o => MessageBox.Show("Clicked " + o), AndCondition);

            OtherCommand = new ConditionRelayCommand<string>(o => MessageBox.Show("Clicked " + o), IsDoorClosedCondition);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDoorClosed
        {
            get
            {
                return _isDoorClosed;
            }
            set
            {
                if (value.Equals(_isDoorClosed))
                {
                    return;
                }
                _isDoorClosed = value;
                OnPropertyChanged();
            }
        }

        public bool IsMotorRunning
        {
            get
            {
                return _isMotorRunning;
            }
            set
            {
                if (value.Equals(_isMotorRunning))
                {
                    return;
                }
                _isMotorRunning = value;
                OnPropertyChanged();
            }
        }

        public Condition IsDoorClosedCondition { get; }

        public Condition IsMotorRunningCondition { get; }

        public Condition AndCondition { get; }

        public ICondition NegatedCondition { get; }

        public ConditionRelayCommand<string> StartCommand { get; private set; }

        public ConditionRelayCommand<string> OtherCommand { get; private set; }

        public IEnumerable<ICondition> Conditions => _conditions;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
