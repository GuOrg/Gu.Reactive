namespace Gu.Wpf.Reactive
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public interface IReadonlyObservableCollection<out T> : IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }
}