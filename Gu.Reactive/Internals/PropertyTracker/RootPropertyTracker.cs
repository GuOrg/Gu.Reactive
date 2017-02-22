namespace Gu.Reactive.Internals
{
    using System;
    using System.ComponentModel;

    internal sealed class RootPropertyTracker : IPathPropertyTracker
    {
        private static readonly PropertyChangedEventArgs ValueChangedEventArgs = new PropertyChangedEventArgs(nameof(Value));
        private static readonly PropertyChangedEventArgs SourceChangedEventArgs = new PropertyChangedEventArgs(nameof(Source));
        private readonly WeakReference sourceRef = new WeakReference(null);

        public RootPropertyTracker(INotifyPropertyChanged value)
        {
            this.sourceRef.Target = value;
        }

        public event PropertyChangedEventHandler TrackedPropertyChanged;

        public INotifyPropertyChanged Source => this.Value as INotifyPropertyChanged;

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

        object IPathPropertyTracker.Value()
        {
            return this.Value;
        }

        public void Dispose()
        {
            // NOP
        }

        private void OnPropertyChanged(PropertyChangedEventArgs e) => this.TrackedPropertyChanged?.Invoke(this, e);
    }
}