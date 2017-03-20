namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// A collection that notifies about changes for binding in WPF.
    /// </summary>
    public interface IObservableCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Move item at oldIndex to newIndex.
        /// </summary>
        void Move(int oldIndex, int newIndex);
    }
}
