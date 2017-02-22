namespace Gu.Reactive.PropertyPathStuff
{
    using System;
    using System.ComponentModel;

    internal sealed class RootPropertyTracker : IPathPropertyTracker
    {
        private static readonly PropertyChangedEventArgs PropertyChangedEventArgs = new PropertyChangedEventArgs(null);
        private static readonly PropertyChangedEventArgs ValueChangedEventArgs = new PropertyChangedEventArgs(nameof(Value));
        private static readonly PropertyChangedEventArgs SourceChangedEventArgs = new PropertyChangedEventArgs(nameof(Source));
        private readonly WeakReference sourceRef = new WeakReference(null);

        public RootPropertyTracker(INotifyPropertyChanged value)
        {
            this.sourceRef.Target = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public INotifyPropertyChanged Source => this.Value as INotifyPropertyChanged;

        PropertyChangedEventArgs IPathPropertyTracker.PropertyChangedEventArgs => PropertyChangedEventArgs;

        PathProperty IPathPropertyTracker.PathProperty => null;

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