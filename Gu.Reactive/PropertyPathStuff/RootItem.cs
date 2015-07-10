namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.ComponentModel;

    internal sealed class RootItem : INotifyingPathItem
    {
        private static readonly PropertyChangedEventArgs _propertyChangedEventArgs = new PropertyChangedEventArgs(null);
        private static readonly PropertyChangedEventArgs ValueChangedEventArgs = new PropertyChangedEventArgs(NameOf.Property<RootItem>(x => x.Value));
        private static readonly PropertyChangedEventArgs SourceChangedEventArgs = new PropertyChangedEventArgs(NameOf.Property<RootItem>(x => x.Source));
        private readonly WeakReference _sourceRef = new WeakReference(null);

        public RootItem(INotifyPropertyChanged value)
        {
            _sourceRef.Target = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public PropertyChangedEventArgs PropertyChangedEventArgs
        {
            get { return _propertyChangedEventArgs; }
        }

        PathProperty INotifyingPathItem.PathProperty
        {
            get { return null; }
        }

        public INotifyPropertyChanged Source
        {
            get
            {
                return Value as INotifyPropertyChanged;
            }
        }

        public bool IsLast
        {
            get { return false; }
        }

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

        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}