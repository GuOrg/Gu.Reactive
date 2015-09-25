namespace Gu.Reactive.Demo
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Demo.Annotations;

    [DebuggerDisplay("{GetType().Name} Value: {Value} Index: {Index}")]
    public class MappedVm : INotifyPropertyChanged
    {
        private int _value;

        private int? _index;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Value
        {
            get { return _value; }
            set
            {
                if (value == _value)
                {
                    return;
                }
                _value = value;
                OnPropertyChanged();
            }
        }

        public int? Index
        {
            get { return _index; }
            set
            {
                if (value == _index)
                {
                    return;
                }
                _index = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MappedVm UpdateIndex(int i)
        {
            Index = i;
            return this;
        }
    }
}