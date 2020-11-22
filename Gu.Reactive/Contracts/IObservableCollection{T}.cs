namespace Gu.Reactive
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// A collection that notifies about changes for binding in WPF.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public interface IObservableCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Move item at oldIndex to newIndex.
        /// </summary>
        /// <param name="oldIndex">The old index.</param>
        /// <param name="newIndex">The new index.</param>
        void Move(int oldIndex, int newIndex);
    }
}
