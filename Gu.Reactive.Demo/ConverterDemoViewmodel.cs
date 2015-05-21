namespace Gu.Reactive.Demo
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;

    public class ConverterDemoViewmodel : INotifyPropertyChanged
    {
        private double _doubleValue = 10;
        private int _intValue = 10;
        private bool? _isVisible;
        private StringComparison _stringComparison;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool? IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (Equals(value, _isVisible))
                {
                    return;
                }
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public double DoubleValue
        {
            get { return _doubleValue; }
            set
            {
                if (value == _doubleValue)
                {
                    return;
                }
                _doubleValue = value;
                OnPropertyChanged();
            }
        }

        public int IntValue
        {
            get { return _intValue; }
            set
            {
                if (value == _intValue)
                {
                    return;
                }
                _intValue = value;
                OnPropertyChanged();
            }
        }

        public StringComparison StringComparison
        {
            get { return _stringComparison; }
            set
            {
                if (value == _stringComparison)
                {
                    return;
                }
                _stringComparison = value;
                OnPropertyChanged();
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
