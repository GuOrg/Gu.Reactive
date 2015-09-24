namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public interface IEditableListView<T> : IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
    }
}
