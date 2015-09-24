namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public interface IReadOnlyObservableCollection<out T> : IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }
}