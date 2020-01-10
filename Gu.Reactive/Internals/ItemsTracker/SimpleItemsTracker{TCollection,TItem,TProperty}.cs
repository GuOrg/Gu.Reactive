namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal sealed class SimpleItemsTracker<TCollection, TItem, TProperty> : ItemsTracker<TCollection, TItem, TProperty>
        where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
        where TItem : class, INotifyPropertyChanged
    {
        private readonly Getter<TItem, TProperty> getter;
        private readonly IdentitySet<TItem> set = IdentitySet.Borrow<TItem>();

        private TCollection? source;
        private bool disposed;

        internal SimpleItemsTracker(TCollection? source, Getter<TItem, TProperty> getter)
        {
            this.getter = getter;
            if (source != null)
            {
                this.UpdateSource(source);
            }
        }

        internal override void UpdateSource(TCollection? newSource)
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
                    this.RemoveItems(this.source);
                }

                this.source = newSource;
                if (newSource is null)
                {
                    foreach (var item in this.set)
                    {
                        item.PropertyChanged -= this.OnItemPropertyChanged;
                    }

                    this.set.Clear();
                }
                else
                {
                    this.AddItems(newSource);
                    newSource.CollectionChanged += this.OnSourceChanged;
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
                        this.RemoveItems(e.OldItems);
                        break;
                    case NotifyCollectionChangedAction.Replace
                        when e.OldItems.TrySingle(out var oldItem) &&
                             e.NewItems.TrySingle(out var newItem) &&
                             ReferenceEquals(oldItem, newItem):
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        this.RemoveItems(e.OldItems);
                        this.AddItems(e.NewItems);
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

        private void AddItems(IEnumerable? items)
        {
            if (items is null)
            {
                return;
            }

            foreach (TItem? item in items)
            {
                //// Signaling initial before subscribing here to get the events in correct order
                //// This can't be made entirely thread safe as an event can be raised on source between signal initial & subscribe.
                this.OnTrackedItemChanged(
                    item,
                    this.source,
                    CachedEventArgs.StringEmpty,
                    new SourceAndValue<INotifyPropertyChanged?, TProperty>(item, this.getter.GetMaybe(item)));

                if (item != null &&
                    !this.set.Contains(item))
                {
                    item.PropertyChanged += this.OnItemPropertyChanged;
                    this.set.Add(item);
                }
            }
        }

        private void RemoveItems(IEnumerable items)
        {
            this.set.IntersectWith(this.source.NotNull());
            foreach (TItem? item in items)
            {
                this.OnTrackedItemChanged(
                    null,
                    this.source,
                    CachedEventArgs.StringEmpty,
                    new SourceAndValue<INotifyPropertyChanged?, TProperty>(item, this.getter.GetMaybe(item)));

                if (item != null &&
                    !this.set.Contains(item))
                {
                    item.PropertyChanged -= this.OnItemPropertyChanged;
                }
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
                        (INotifyPropertyChanged?)sender,
                        this.getter.GetMaybe(item)));
            }
        }
    }
}
