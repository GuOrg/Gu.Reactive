namespace Gu.Reactive.Tests
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

    public class FakeInpc : INotifyPropertyChanged
    {
        private bool? _prop1;
        private bool _prop2;
        private Level _next;
        private string _name;

        public event PropertyChangedEventHandler PropertyChanged;
        public bool? Prop1
        {
            get
            {
                return _prop1;
            }
            set
            {
                if (value.Equals(_prop1))
                {
                    return;
                }
                _prop1 = value;
                this.OnPropertyChanged();
            }
        }
        public bool Prop2
        {
            get
            {
                return _prop2;
            }
            set
            {
                if (value.Equals(_prop2))
                {
                    return;
                }
                _prop2 = value;
                this.OnPropertyChanged();
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
                this.OnPropertyChanged();
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
                this.OnPropertyChanged();
            }
        }

        public Level Method()
        {
            return Next;
        }

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}