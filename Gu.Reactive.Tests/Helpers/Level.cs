namespace Gu.Reactive.Tests
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

    public class Level : INotifyPropertyChanged
    {
        private bool _isTrue;
        private Level _next;
        private string _name;

        private bool? _nullableValue;

        private int _value;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsTrue
        {
            get
            {
                return _isTrue;
            }
            set
            {
                if (value.Equals(_isTrue))
                {
                    return;
                }
                _isTrue = value;
                OnPropertyChanged();
            }
        }

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value == _value)
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

        public Level Method()
        {
            return Next;
        }
    }

    public struct StructLevel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; private set; }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}