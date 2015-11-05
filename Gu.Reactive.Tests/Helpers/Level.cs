namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class Level : INotifyPropertyChanged
    {
        private bool _isTrue;
        private Level _next;
        private string _name;

        private bool? _isTrueOrNull;

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

        public bool? IsTrueOrNull
        {
            get
            {
                return _isTrueOrNull;
            }
            set
            {
                if (Equals(value, _isTrueOrNull))
                {
                    return;
                }
                _isTrueOrNull = value;
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

        public NotInpc NotInpc { get; private set; }

        public Level Method()
        {
            return Next;
        }

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}