namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// A set that notifies about changes.
    /// </summary>
    public interface IObservableSet<T> : ISet<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
    }
}