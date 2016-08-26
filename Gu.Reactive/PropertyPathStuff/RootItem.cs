namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.ComponentModel;

    internal sealed class RootItem : INotifyingPathItem
    {
        private static readonly PropertyChangedEventArgs PropertyChangedEventArgs = new PropertyChangedEventArgs(null);
        private static readonly PropertyChangedEventArgs ValueChangedEventArgs = new PropertyChangedEventArgs(nameof(Value));
        private static readonly PropertyChangedEventArgs SourceChangedEventArgs = new PropertyChangedEventArgs(nameof(Source));
        private readonly WeakReference sourceRef = new WeakReference(null);

        public RootItem(INotifyPropertyChanged value)
        {
            this.sourceRef.Target = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        PropertyChangedEventArgs INotifyingPathItem.PropertyChangedEventArgs => PropertyChangedEventArgs;

        PathProperty INotifyingPathItem.PathProperty => null;

        public INotifyPropertyChanged Source => this.Value as INotifyPropertyChanged;

        public bool IsLast => false;

        public object Value
        {
            get
            {
                return this.sourceRef.Target;

            }

            set
            {
                if (ReferenceEquals(this.sourceRef.Target, value))
                {
                    return;
                }

                this.sourceRef.Target = value;
                this.OnPropertyChanged(ValueChangedEventArgs);
                this.OnPropertyChanged(SourceChangedEventArgs);
            }
        }

        public void Dispose()
        {
            // NOP
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e) => this.PropertyChanged?.Invoke(this, e);
    }
}