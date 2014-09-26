namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Demo.Annotations;

    public class ViewModel : INotifyPropertyChanged
    {
        private bool _isOk;

        public ViewModel()
        {
            Condition = new Condition(this.ToObservable(x => x.IsOk), () => IsOk){Name = "Condition"};
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsOk
        {
            get
            {
                return this._isOk;
            }
            set
            {
                if (value.Equals(this._isOk))
                {
                    return;
                }
                this._isOk = value;
                this.OnPropertyChanged();
            }
        }

        public Condition Condition { get; private set; }

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
