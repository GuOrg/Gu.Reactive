namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;

    using JetBrains.Annotations;

    public class NinjaBindingViewModel : INotifyPropertyChanged
    {
        private bool visible = true;

        private Visibility visibility;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Visible
        {
            get => this.visible;

            set
            {
                if (value.Equals(this.visible))
                {
                    return;
                }

                this.visible = value;
                this.OnPropertyChanged();
                this.Visibility = this.visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility Visibility
        {
            get => this.visibility;

            set
            {
                if (value == this.visibility)
                {
                    return;
                }

                this.visibility = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
