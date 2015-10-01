namespace Gu.Reactive
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public interface IObservableSet<T> : ISet<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
    }
}