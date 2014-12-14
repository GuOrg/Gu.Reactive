namespace Gu.Reactive.Demo
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    using Gu.Reactive.Demo.Annotations;

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
                Visibility = _visible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
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
