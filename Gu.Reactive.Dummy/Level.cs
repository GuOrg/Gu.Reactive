namespace Gu.Reactive.Dummy
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public sealed class Level : INotifyPropertyChanged
    {
        private Level next;
        private int value;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (value == this.value)
                {
                    return;
                }

                this.value = value;
                this.OnPropertyChanged();
            }
        }

        public Level Next
        {
            get
            {
                return this.next;
            }

            set
            {
                if (Equals(value, this.next))
                {
                    return;
                }

                this.next = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}