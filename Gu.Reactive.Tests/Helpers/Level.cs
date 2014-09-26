namespace Gu.Reactive.Tests
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

    public class Level : INotifyPropertyChanged
    {
        private bool _value;
        private Level _next;
        public event PropertyChangedEventHandler PropertyChanged;
        public bool Value
        {
            get
            {
                return this._value;
            }
            set
            {
                if (value.Equals(this._value))
                {
                    return;
                }
                this._value = value;
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