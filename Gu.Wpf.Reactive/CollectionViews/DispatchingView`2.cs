namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public class DispatchingView<TCollection, TItem> : ObservableCollectionWrapperBase<TCollection, TItem>
        where TCollection : IList<TItem>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public DispatchingView(TCollection inner)
            : base(inner)
        {
            InnerCollectionChangedObservable.Subscribe(x => OnCollectionChanged(x.EventArgs));
            InnerCountChangedObservable.Subscribe(x => OnPropertyChanged(x.EventArgs));
            InnerIndexerChangedObservable.Subscribe(x => OnPropertyChanged(x.EventArgs));
        }
    }
}
