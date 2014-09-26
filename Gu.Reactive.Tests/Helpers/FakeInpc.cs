namespace Gu.Reactive.Tests
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

    public class FakeInpc : INotifyPropertyChanged
    {
        private bool _prop1;
        private bool _prop2;
        private Level _next;
        private string _name;

        public event PropertyChangedEventHandler PropertyChanged;
        public bool Prop1
        {
            get
            {
                return this._prop1;
            }
            set
            {
                if (value.Equals(this._prop1))
                {
                    return;
                }
                this._prop1 = value;
                this.OnPropertyChanged();
            }
        }
        public bool Prop2
        {
            get
            {
                return this._prop2;
            }
            set
            {
                if (value.Equals(this._prop2))
                {
                    return;
                }
                this._prop2 = value;
                this.OnPropertyChanged();
            }
        }
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                if (value == this._name)
                {
                    return;
                }
                this._name = value;
                this.OnPropertyChanged();
            }
        }
        public Level Next
        {
            get
            {
                return this._next;
            }
            set
            {
                if (Equals(value, this._next))
                {
                    return;
                }
                this._next = value;
                this.OnPropertyChanged();
            }
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