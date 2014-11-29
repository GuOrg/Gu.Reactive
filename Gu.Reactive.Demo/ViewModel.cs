namespace Gu.Reactive.Demo
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using Gu.Reactive.Demo.Annotations;
    using Gu.Wpf.Reactive;

    using Condition = Gu.Reactive.Condition;

    public class ViewModel : INotifyPropertyChanged
    {
        private readonly List<ICondition> _conditions = new List<ICondition>();
        private bool _isDoorClosed;
        private bool _isMotorRunning;

        public ViewModel()
        {
            this.Condition1 = new Condition(
                this.ToObservable(x => x.IsDoorClosed), 
                () => this.IsDoorClosed) { Name = "Door open" };

            this.Condition2 = new Condition(
                this.ToObservable(x => x.IsMotorRunning), 
                () => this.IsMotorRunning) { Name = "Motor running" };

            DependingCondition = new AndCondition(Condition1, Condition2)
            {
                Name = "Can start"
            };

            NegatedCondition1 = Condition1.Negate();

            _conditions = new List<ICondition>
            {
                Condition1, 
                Condition2, 
                DependingCondition, 
                NegatedCondition1
            };

            StartCommand = new ConditionRelayCommand(
                o => MessageBox.Show("Clicked " + o),
                DependingCondition)
            {
                ToolTipText = "Start the thing"
            };

            OtherCommand = new ConditionRelayCommand(
                o => MessageBox.Show("Clicked " + o),
                Condition1)
            {
                ToolTipText = "Another command"
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDoorClosed
        {
            get
            {
                return this._isDoorClosed;
            }
            set
            {
                if (value.Equals(this._isDoorClosed))
                {
                    return;
                }
                this._isDoorClosed = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsMotorRunning
        {
            get
            {
                return this._isMotorRunning;
            }
            set
            {
                if (value.Equals(this._isMotorRunning))
                {
                    return;
                }
                this._isMotorRunning = value;
                this.OnPropertyChanged();
            }
        }

        public Condition Condition1 { get; private set; }

        public Condition Condition2 { get; private set; }

        public Condition DependingCondition { get; private set; }

        public ICondition NegatedCondition1 { get; private set; }

        public ConditionRelayCommand StartCommand { get; private set; }

        public ConditionRelayCommand OtherCommand { get; private set; }

        public IEnumerable<ICondition> Conditions
        {
            get
            {
                return _conditions;
            }
        }

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
