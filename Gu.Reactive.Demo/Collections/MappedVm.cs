namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    [DebuggerDisplay("{GetType().Name} Value: {Value} Index: {Index}")]
    public class MappedVm : INotifyPropertyChanged
    {
        private int value;

        private int? index;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get { return this.value; }

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
            get { return this.index; }

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

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MappedVm UpdateIndex(int i)
        {
            this.Index = i;
            return this;
        }
    }
}