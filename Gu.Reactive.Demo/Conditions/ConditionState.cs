namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class ConditionState : INotifyPropertyChanged
    {
        public static readonly ConditionState Instance = new ConditionState();
        private bool _isLeftDoorOpen;
        private bool _isRightDoorOpen;
        private bool _isBackDoorOpen;
        private bool _isMotorRunning;
        private double _fuelLevel = 6;

        private ConditionState()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsLeftDoorOpen
        {
            get
            {
                return _isLeftDoorOpen;
            }
            set
            {
                if (value.Equals(_isLeftDoorOpen))
                {
                    return;
                }
                _isLeftDoorOpen = value;
                OnPropertyChanged();
            }
        }

        public bool IsRightDoorOpen
        {
            get { return _isRightDoorOpen; }
            set
            {
                if (value == _isRightDoorOpen)
                {
                    return;
                }
                _isRightDoorOpen = value;
                OnPropertyChanged();
            }
        }

        public bool IsBackDoorOpen
        {
            get { return _isBackDoorOpen; }
            set
            {
                if (value == _isBackDoorOpen)
                {
                    return;
                }
                _isBackDoorOpen = value;
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

        public double FuelLevel
        {
            get { return _fuelLevel; }
            set
            {
                if (value.Equals(_fuelLevel))
                {
                    return;
                }
                _fuelLevel = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}