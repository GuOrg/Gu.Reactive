namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal sealed class SimpleItemsTrackerSlim<TCollection, TItem, TProperty> : ItemsTrackerSlim
        where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
        where TItem : class, INotifyPropertyChanged
    {
        private readonly object gate = new object();
        private readonly TCollection source;
        private readonly Getter<TItem, TProperty> getter;
        private readonly IdentitySet<TItem> set = IdentitySet.Borrow<TItem>();

        public SimpleItemsTrackerSlim(TCollection source, Getter<TItem, TProperty> getter)
        {
            this.source = source;
            this.getter = getter;
            source.CollectionChanged += this.OnSourceChanged;
            this.UpdateSubscriptions(null, source);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Disposed)
                {
                    return;
                }

                this.source.CollectionChanged -= this.OnSourceChanged;
                lock (this.gate)
                {
                    if (this.Disposed)
                    {
                        return;
                    }

                    foreach (var item in this.set)
                    {
                        item.PropertyChanged -= this.OnItemPropertyChanged;
                    }

                    IdentitySet.Return(this.set);
                    base.Dispose(true);
                }
            }
        }

        private void OnSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.OnItemPropertyChanged(CachedEventArgs.StringEmpty);
                    this.UpdateSubscriptions(e.OldItems, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.OnItemPropertyChanged(CachedEventArgs.StringEmpty);
                    this.UpdateSubscriptions(e.OldItems, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Replace when e.OldItems.TrySingle(out var oldItem) &&
                                                                e.NewItems.TrySingle(out var newItem) &&
                                                                ReferenceEquals(oldItem, newItem):
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.OnItemPropertyChanged(CachedEventArgs.StringEmpty);
                    this.UpdateSubscriptions(e.OldItems, e.NewItems);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.OnItemPropertyChanged(CachedEventArgs.StringEmpty);
                    //// ReSharper disable once InconsistentlySynchronizedField
                    this.UpdateSubscriptions(this.set, this.source);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(e), e, "Unknown NotifyCollectionChangedAction.");
            }
        }

        private void UpdateSubscriptions(IEnumerable oldItems, IEnumerable newItems)
        {
            if (this.Disposed)
            {
                return;
            }

            lock (this.gate)
            {
                if (this.Disposed)
                {
                    return;
                }

                if (newItems != null)
                {
                    foreach (TItem item in newItems)
                    {
                        if (!(item is null) &&
                            this.set.Add(item))
                        {
                            item.PropertyChanged += this.OnItemPropertyChanged;
                        }
                    }
                }

                if (oldItems != null)
                {
                    this.set.IntersectWith(this.source);
                    foreach (TItem item in oldItems)
                    {
                        if (!(item is null) &&
                            !this.set.Contains(item))
                        {
                            item.PropertyChanged -= this.OnItemPropertyChanged;
                        }
                    }
                }
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.IsMatch(this.getter.Property))
            {
                this.OnItemPropertyChanged(e);
            }
        }
    }
}
