namespace Gu.Reactive.Demo
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;
    using Gu.Wpf.Reactive;

    public class ConditionsViewModel : INotifyPropertyChanged
    {
        private readonly List<ICondition> _conditions;
        public static readonly ConditionsViewModel Instance = new ConditionsViewModel();

        private ConditionsViewModel()
        {
            _conditions = new List<ICondition>
            {
                IsLeftDoorOpenCondition,
                IsLeftDoorClosedCondition,
                IsMotorRunningCondition,
                new IsAnyDoorOpen(),
                new IsAnyDoorOpen().Negate(),
                CanStartCondition,
                CanStartCondition.Negate(),
                new SyncErrorCondition()
            };

            StartCommand = new ConditionRelayCommand(() => ConditionState.Instance.IsMotorRunning = true, CanStartCondition);

            StopCommand = new ConditionRelayCommand(() => ConditionState.Instance.IsMotorRunning = false, CanStopCondition);
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

        public IEnumerable<ICondition> Conditions => _conditions;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
