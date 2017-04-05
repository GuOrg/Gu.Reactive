namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public abstract class AbstractFake : INotifyPropertyChanged
    {
        private int baseValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public int BaseValue
        {
            get => this.baseValue;

            set
            {
                if (value == this.baseValue)
                {
                    return;
                }

                this.baseValue = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}