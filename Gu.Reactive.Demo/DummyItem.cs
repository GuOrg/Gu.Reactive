namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    public class DummyItem : INotifyPropertyChanged
    {
        private int value;

        public DummyItem()
        {
        }

        public DummyItem(int value)
        {
            this.Value = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get => this.value;

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

        public override string ToString()
        {
            return this.Value.ToString();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
