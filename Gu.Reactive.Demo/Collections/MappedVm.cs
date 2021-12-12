namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    [DebuggerDisplay("{GetType().Name} Value: {Value} Index: {Index}")]
    public class MappedVm : INotifyPropertyChanged
    {
        private int value;

        private int? index;

        public event PropertyChangedEventHandler? PropertyChanged;

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

        public int? Index
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

        public MappedVm UpdateIndex(int i)
        {
            this.Index = i;
            return this;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
