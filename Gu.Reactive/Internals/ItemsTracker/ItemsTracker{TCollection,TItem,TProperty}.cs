namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal abstract class ItemsTracker<TCollection, TItem, TProperty> : IDisposable
        where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
        where TItem : class, INotifyPropertyChanged
    {
        protected readonly object Gate = new object();

        internal event TrackedItemPropertyChangedEventHandler<TItem, TProperty> TrackedItemChanged;

        protected bool HasSubscribers => this.TrackedItemChanged != null;

        public void Dispose()
        {
            this.Dispose(true);
        }

        internal abstract void UpdateSource(TCollection newSource);

        protected virtual void Dispose(bool disposing)
        {
            // nop
        }

        protected void OnTrackedItemChanged(TItem item, object sender, PropertyChangedEventArgs e, SourceAndValue<INotifyPropertyChanged, TProperty> sourceAndValue)
        {
            this.TrackedItemChanged?.Invoke(item, sender, e, sourceAndValue);
        }
    }
}