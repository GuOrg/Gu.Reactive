namespace Gu.Reactive.Tests.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class Indexed<T> : INotifyPropertyChanged
    {
        private int index;

        public Indexed(T item, int index)
        {
            this.Item = item;
            this.index = index;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public T Item { get;  }

        public int Index
        {
            get => this.index;

            set
            {
                if (value == this.index)
                {
                    return;
                }

                this.index = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
