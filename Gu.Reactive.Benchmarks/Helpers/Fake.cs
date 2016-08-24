namespace Gu.Reactive.Benchmarks
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class Fake : INotifyPropertyChanged, IFake
    {
        private bool _isTrue;
        private Level _next;
        private string _name;

        private StructLevel _structLevel;

        private int _value;

        public event PropertyChangedEventHandler PropertyChanged;
       
        public int WriteOnly { set { return; } }
      
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

        public StructLevel StructLevel
        {
            get
            {
                return _structLevel;
            }
            set
            {
                _structLevel = value;
                OnPropertyChanged();
            }
        }

        public NotInpc NotInpc { get; private set; }

        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public Level Method()
        {
            return Next;
        }

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}