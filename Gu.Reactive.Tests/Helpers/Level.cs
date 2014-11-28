namespace Gu.Reactive.Tests
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

    public class Level : INotifyPropertyChanged
    {
        private bool _value;
        private Level _next;
        private string _name;

        private bool? _nullableValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value.Equals(_value))
                {
                    return;
                }
                _value = value;
                OnPropertyChanged();
            }
        }

        public bool? NullableValue
        {
            get
            {
                return _nullableValue;
            }
            set
            {
                if (Equals(value, _nullableValue))
                {
                    return;
                }
                _nullableValue = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value == _name)
                {
                    return;
                }
                _name = value;
                OnPropertyChanged();
            }
        }

        public Level Next
        {
            get
            {
                return _next;
            }
            set
            {
                if (Equals(value, _next))
                {
                    return;
                }
                _next = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}