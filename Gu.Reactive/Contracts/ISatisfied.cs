namespace Gu.Reactive
{
    using System.ComponentModel;

    public interface ISatisfied : INotifyPropertyChanged
    {
        bool? IsSatisfied { get; }
    }
}