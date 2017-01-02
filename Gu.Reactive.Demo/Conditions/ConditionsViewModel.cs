namespace Gu.Reactive.Demo
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Wpf.Reactive;
    using JetBrains.Annotations;

    public class ConditionsViewModel : INotifyPropertyChanged
    {
        private readonly List<ICondition> conditions;
        public static readonly ConditionsViewModel Instance = new ConditionsViewModel();

        private ConditionsViewModel()
        {
            this.conditions = new List<ICondition>
            {
                this.IsLeftDoorOpenCondition,
                this.IsLeftDoorClosedCondition,
                this.IsMotorRunningCondition,
                new IsAnyDoorOpen(),
                new IsAnyDoorOpen().Negate(),
                this.CanStartCondition,
                this.CanStartCondition.Negate(),
                new SyncErrorCondition()
            };

            this.StartCommand = new ConditionRelayCommand(() => ConditionState.Instance.IsMotorRunning = true, this.CanStartCondition);

            this.StopCommand = new ConditionRelayCommand(() => ConditionState.Instance.IsMotorRunning = false, this.CanStopCondition);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICondition IsLeftDoorOpenCondition { get; } = new IsLeftDoorOpen();

        public ICondition IsRightDoorOpenCondition { get; } = new IsRightDoorOpen();

        public ICondition IsBackDoorOpenCondition { get; } = new IsBackDoorOpen();

        public ICondition IsLeftDoorClosedCondition { get; } = new IsLeftDoorOpen().Negate();

        public ICondition IsMotorRunningCondition { get; } = new IsMotorRunning();

        public ICondition CanStartCondition { get; } = new CanStart();

        public ICondition CanStopCondition { get; } = new IsMotorRunning();

        public ConditionRelayCommand StartCommand { get;  }

        public ConditionRelayCommand StopCommand { get;  }

        public IEnumerable<ICondition> Conditions => this.conditions;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
