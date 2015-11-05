namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using JetBrains.Annotations;

    public class NinjaBindingViewModel : INotifyPropertyChanged
    {
        private bool _visible = true;

        private Visibility _visibility;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                if (value.Equals(_visible))
                {
                    return;
                }
                _visible = value;
                OnPropertyChanged();
                Visibility = _visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                if (value == _visibility)
                {
                    return;
                }
                _visibility = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
