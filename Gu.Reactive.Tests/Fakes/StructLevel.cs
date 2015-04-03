namespace Gu.Reactive.Tests.Fakes
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Reactive.Annotations;

    public struct StructLevel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; private set; }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}