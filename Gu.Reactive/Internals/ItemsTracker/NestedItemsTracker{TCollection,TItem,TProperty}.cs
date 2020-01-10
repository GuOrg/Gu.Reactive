namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    internal sealed class NestedItemsTracker<TCollection, TItem, TProperty> : ItemsTracker<TCollection, TItem, TProperty>
        where TCollection : class, IEnumerable<TItem?>, INotifyCollectionChanged
        where TItem : class, INotifyPropertyChanged
    {
        private readonly NotifyingPath<TItem, TProperty> path;

        private readonly IdentityMap<TItem, PropertyPathTracker<TItem, TProperty>> map = IdentityMap.Borrow<TItem, PropertyPathTracker<TItem, TProperty>>();

        private TCollection? source;
        private bool disposed;

        internal NestedItemsTracker(TCollection? source, NotifyingPath<TItem, TProperty> path)
        {
            this.path = path;
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
                    foreach (var kvp in this.map)
                    {
                        kvp.Value.TrackedPropertyChanged -= this.OnItemPropertyChanged;
                        kvp.Value.Dispose();
                    }

                    this.map.Clear();
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

                    foreach (var kvp in this.map)
                    {
                        kvp.Value.TrackedPropertyChanged -= this.OnItemPropertyChanged;
                        kvp.Value.Dispose();
                    }

                    IdentityMap.Return(this.map);
                }
            }

            base.Dispose(disposing);
        }

        private void OnItemPropertyChanged(IPropertyTracker tracker, object sender, PropertyChangedEventArgs e, SourceAndValue<INotifyPropertyChanged?, TProperty> sourceAndValue)
        {
            this.OnTrackedItemChanged(
                ((PropertyPathTracker<TItem, TProperty>)tracker.PathTracker).Source,
                sender,
                e,
                sourceAndValue);
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
                        this.RemoveItems(this.map.Keys);
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
                    this.path.SourceAndValue(item));

                if (item != null &&
                    !this.map.ContainsKey(item))
                {
                    var tracker = this.path.CreateTracker(item);
                    tracker.TrackedPropertyChanged += this.OnItemPropertyChanged;
                    this.map.Add(item, tracker);
                }
            }
        }

        private void RemoveItems(IEnumerable items)
        {
            var set = IdentitySet.Borrow<TItem>();
            foreach (TItem? item in items)
            {
                this.OnTrackedItemChanged(
                    null,
                    this.source,
                    CachedEventArgs.StringEmpty,
                    this.path.SourceAndValue(item));

                if (item != null)
                {
                    set.Add(item);
                }
            }

            set.ExceptWith(this.source.NotNull());
            foreach (var item in set)
            {
                if (item is null)
                {
                    continue;
                }

                var tracker = this.map[item];
                tracker.TrackedPropertyChanged -= this.OnItemPropertyChanged;
                tracker.Dispose();
                this.map.Remove(item);
            }

            IdentitySet.Return(set);
        }
    }
}
