namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class ConditionState : INotifyPropertyChanged
    {
        public static readonly ConditionState Instance = new ConditionState();
        private bool isLeftDoorOpen;
        private bool isRightDoorOpen;
        private bool isBackDoorOpen;
        private bool isMotorRunning;
        private double fuelLevel = 6;

        private ConditionState()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsLeftDoorOpen
        {
            get => this.isLeftDoorOpen;

            set
            {
                if (value.Equals(this.isLeftDoorOpen))
                {
                    return;
                }

                this.isLeftDoorOpen = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsRightDoorOpen
        {
            get => this.isRightDoorOpen;

            set
            {
                if (value == this.isRightDoorOpen)
                {
                    return;
                }

                this.isRightDoorOpen = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsBackDoorOpen
        {
            get => this.isBackDoorOpen;

            set
            {
                if (value == this.isBackDoorOpen)
                {
                    return;
                }

                this.isBackDoorOpen = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsMotorRunning
        {
            get => this.isMotorRunning;

            set
            {
                if (value.Equals(this.isMotorRunning))
                {
                    return;
                }

                this.isMotorRunning = value;
                this.OnPropertyChanged();
            }
        }

        public double FuelLevel
        {
            get => this.fuelLevel;

            set
            {
                if (value.Equals(this.fuelLevel))
                {
                    return;
                }

                this.fuelLevel = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}