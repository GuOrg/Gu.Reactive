namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    internal sealed class SimpleItemsTracker<TCollection, TItem, TProperty> : ItemsTracker<TCollection, TItem, TProperty>
        where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
        where TItem : class, INotifyPropertyChanged
    {
        private readonly Getter<TItem, TProperty> getter;
        private readonly IdentitySet<TItem> set = IdentitySet.Borrow<TItem>();

        private TCollection source;
        private bool disposed;

        public SimpleItemsTracker(TCollection source, Getter<TItem, TProperty> getter)
        {
            this.getter = getter;
            if (source != null)
            {
                this.UpdateSource(source);
            }
        }

        internal override void UpdateSource(TCollection newSource)
        {
            if (this.disposed ||
                ReferenceEquals(this.source, newSource))
            {
                return;
            }

            lock (this.Gate)
            {
                if (this.disposed ||
                    ReferenceEquals(this.source, newSource))
                {
                    return;
                }

                if (this.source != null)
                {
                    this.source.CollectionChanged -= this.OnSourceChanged;
                }

                this.source = newSource;
                if (newSource == null)
                {
                    foreach (var item in this.set)
                    {
                        item.PropertyChanged -= this.OnItemPropertyChanged;
                    }

                    this.set.Clear();
                }
                else
                {
                    this.source.CollectionChanged += this.OnSourceChanged;
                    this.OnSourceChanged(this.source, CachedEventArgs.NotifyCollectionReset);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                if (this.disposed)
                {
                    return;
                }

                lock (this.Gate)
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    this.disposed = true;
                    var collection = this.source;
                    if (collection != null)
                    {
                        collection.CollectionChanged -= this.OnSourceChanged;
                        this.source = null;
                    }

                    foreach (var item in this.set)
                    {
                        item.PropertyChanged -= this.OnItemPropertyChanged;
                    }

                    IdentitySet.Return(this.set);
                }
            }

            base.Dispose(disposing);
        }

        private void OnSourceChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.disposed)
            {
                return;
            }

            lock (this.Gate)
            {
                if (this.disposed)
                {
                    return;
                }

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        this.AddItems(e.NewItems);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        this.RemoveItems(e.OldItems.OfType<TItem>());
                        break;
                    case NotifyCollectionChangedAction.Replace when e.OldItems.TrySingle(out var oldItem) &&
                                                                    e.NewItems.TrySingle(out var newItem) &&
                                                                    ReferenceEquals(oldItem, newItem):
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        this.AddItems(e.NewItems);
                        this.RemoveItems(e.OldItems.OfType<TItem>());
                        if (e.NewItems.All(x => x is null) &&
                            e.OldItems.Any(x => !(x is null)))
                        {
                            foreach (TItem item in e.OldItems)
                            {
                                this.OnTrackedItemChanged(
                                    null,
                                    sender,
                                    CachedEventArgs.StringEmpty,
                                    SourceAndValue.Create(
                                        (INotifyPropertyChanged)item,
                                        this.getter.GetMaybe(item)));
                            }
                        }

                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        this.RemoveItems(this.set);
                        this.AddItems(this.source);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(e));
                }
            }
        }

        private void AddItems(IEnumerable items)
        {
            foreach (TItem item in items)
            {
                if (item == null ||
                    !this.set.Add(item))
                {
                    continue;
                }

                //// Signaling initial before subscribing here to get the events in correct order
                //// This can't be made entirely thread safe as an event can be raised on source between signal initial & subscribe.
                this.SignalInitial(item);
                item.PropertyChanged += this.OnItemPropertyChanged;
            }
        }

        private void RemoveItems(IEnumerable<TItem> items)
        {
            this.set.IntersectWith(this.source);
            foreach (var item in items)
            {
                if (item == null ||
                    this.set.Contains(item))
                {
                    continue;
                }

                item.PropertyChanged -= this.OnItemPropertyChanged;
            }
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.IsMatch(this.getter.Property))
            {
                var item = (TItem)sender;
                this.OnTrackedItemChanged(
                    item,
                    sender,
                    e,
                    SourceAndValue.Create(
                        (INotifyPropertyChanged)sender,
                        this.getter.GetMaybe(item)));
            }
        }

        private void SignalInitial(TItem item)
        {
            if (!this.HasSubscribers)
            {
                return;
            }

            this.OnTrackedItemChanged(
                item,
                item,
                CachedEventArgs.StringEmpty,
                SourceAndValue.Create(
                    (INotifyPropertyChanged)item,
                    this.getter.GetMaybe(item)));
        }
    }
}
