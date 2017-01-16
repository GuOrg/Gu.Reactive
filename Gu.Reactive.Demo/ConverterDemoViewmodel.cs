namespace Gu.Reactive.Demo
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public class ConverterDemoViewmodel : INotifyPropertyChanged
    {
        private double doubleValue = 10;
        private int intValue = 10;
        private bool? isVisible;
        private StringComparison stringComparison;

        private double factor;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool? IsVisible
        {
            get
            {
                return this.isVisible;
            }

            set
            {
                if (value == this.isVisible)
                {
                    return;
                }

                this.isVisible = value;
                this.OnPropertyChanged();
            }
        }

        public double DoubleValue
        {
            get
            {
                return this.doubleValue;
            }

            set
            {
                if (value == this.doubleValue)
                {
                    return;
                }

                this.doubleValue = value;
                this.OnPropertyChanged();
            }
        }

        public double Factor
        {
            get
            {
                return this.factor;
            }

            set
            {
                if (value.Equals(this.factor))
                {
                    return;
                }

                this.factor = value;
                this.OnPropertyChanged();
            }
        }

        public int IntValue
        {
            get
            {
                return this.intValue;
            }

            set
            {
                if (value == this.intValue)
                {
                    return;
                }

                this.intValue = value;
                this.OnPropertyChanged();
            }
        }

        public StringComparison StringComparison
        {
            get
            {
                return this.stringComparison;
            }

            set
            {
                if (value == this.stringComparison)
                {
                    return;
                }

                this.stringComparison = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
