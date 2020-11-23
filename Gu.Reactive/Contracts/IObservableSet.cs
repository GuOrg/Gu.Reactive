namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// A set that notifies about changes.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public interface IObservableSet<T> : ISet<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
    }
}
