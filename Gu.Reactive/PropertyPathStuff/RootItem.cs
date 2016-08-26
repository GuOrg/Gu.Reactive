namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.ComponentModel;

    internal sealed class RootItem : INotifyingPathItem
    {
        private static readonly PropertyChangedEventArgs PropertyChangedEventArgs = new PropertyChangedEventArgs(null);
        private static readonly PropertyChangedEventArgs ValueChangedEventArgs = new PropertyChangedEventArgs(nameof(Value));
        private static readonly PropertyChangedEventArgs SourceChangedEventArgs = new PropertyChangedEventArgs(nameof(Source));
        private readonly WeakReference _sourceRef = new WeakReference(null);

        public RootItem(INotifyPropertyChanged value)
        {
            _sourceRef.Target = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        PropertyChangedEventArgs INotifyingPathItem.PropertyChangedEventArgs => PropertyChangedEventArgs;

        PathProperty INotifyingPathItem.PathProperty => null;

        public INotifyPropertyChanged Source => Value as INotifyPropertyChanged;

        public bool IsLast => false;

        public object Value
        {
            get
            {
                return _sourceRef.Target;

            }
            set
            {
                if (ReferenceEquals(_sourceRef.Target, value))
                {
                    return;
                }

                _sourceRef.Target = value;
                OnPropertyChanged(ValueChangedEventArgs);
                OnPropertyChanged(SourceChangedEventArgs);
            }
        }

        public void Dispose()
        {
            // NOP 
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
    }
}