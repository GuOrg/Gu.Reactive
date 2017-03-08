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
        private readonly HashSet<TItem> set = new HashSet<TItem>(ObjectIdentityComparer<TItem>.Default);

        private TCollection source;

        public SimpleItemsTracker(TCollection source, Getter<TItem, TProperty> getter)
        {
            this.getter = getter;
            if (source != null)
            {
                this.AddItems(source);
                this.UpdateSource(source);
            }
        }

        internal override void UpdateSource(TCollection newSource)
        {
            if (this.Disposed ||
                ReferenceEquals(this.source, newSource))
            {
                return;
            }

            lock (this.Gate)
            {
                if (this.Disposed ||
                    ReferenceEquals(this.source, newSource))
                {
                    return;
                }

                if (this.source != null)
                {
                    this.source.CollectionChanged -= this.OnTrackedCollectionChanged;
                }

                this.source = newSource;
                if (newSource == null)
                {
                    foreach (var item in this.set)
                    {
                        item.PropertyChanged -= this.OnTrackedItemChanged;
                    }

                    this.set.Clear();
                }
                else
                {
                    this.source.CollectionChanged += this.OnTrackedCollectionChanged;
                    this.OnTrackedCollectionChanged(this.source, CachedEventArgs.NotifyCollectionReset);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.Disposed)
            {
                return;
            }

            this.Disposed = true;
            if (disposing)
            {
                if (this.Disposed)
                {
                    return;
                }

                lock (this.Gate)
                {
                    if (this.Disposed)
                    {
                        return;
                    }

                    this.Disposed = true;
                    var collection = this.source;
                    if (collection != null)
                    {
                        collection.CollectionChanged -= this.OnTrackedCollectionChanged;
                        this.source = null;
                    }

                    foreach (var item in this.set)
                    {
                        item.PropertyChanged -= this.OnTrackedItemChanged;
                    }

                    this.set.Clear();
                }
            }

            base.Dispose(disposing);
        }

        private void OnTrackedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Disposed)
            {
                return;
            }

            lock (this.Gate)
            {
                if (this.Disposed)
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
                    case NotifyCollectionChangedAction.Replace:
                        this.AddItems(e.NewItems);
                        this.RemoveItems(e.OldItems.OfType<TItem>());
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        this.RemoveItems(this.set);
                        this.AddItems(this.source);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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

                //// Signaling initial before subscrinbing here to get the events in correct order
                //// This can't be made entirely thread safe as an event can be raised on source bewteen signal initial & subscribe.
                this.SignalInitial(item);
                item.PropertyChanged += this.OnTrackedItemChanged;
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

                item.PropertyChanged -= this.OnTrackedItemChanged;
            }
        }

        private void OnTrackedItemChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = (TItem)sender;
            this.OnTrackedItemChanged(
                item,
                sender,
                e,
                new SourceAndValue<TProperty>(
                    sender,
                    Maybe.Some(this.getter.GetValue(item))));
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
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs(string.Empty),
                new SourceAndValue<TProperty>(
                    item, 
                    Maybe.Some(this.getter.GetValue(item))));
        }
    }
}