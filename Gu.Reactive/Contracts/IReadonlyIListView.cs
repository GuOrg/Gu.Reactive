namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public interface IReadonlyIListView<out T> : IReadOnlyList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
    }
}