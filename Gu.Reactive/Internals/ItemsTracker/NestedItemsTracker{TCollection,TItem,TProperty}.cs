namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    internal sealed class NestedItemsTracker<TCollection, TItem, TProperty> : ItemsTracker<TCollection, TItem, TProperty>
        where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
        where TItem : class, INotifyPropertyChanged
    {
        private readonly NotifyingPath<TItem, TProperty> path;
        private readonly Dictionary<TItem, PropertyPathTracker<TItem, TProperty>> map = new Dictionary<TItem, PropertyPathTracker<TItem, TProperty>>(ObjectIdentityComparer<TItem>.Default);

        private TCollection source;
        private bool disposed;

        public NestedItemsTracker(TCollection source, NotifyingPath<TItem, TProperty> path)
        {
            this.path = path;
            if (source != null)
            {
                this.AddItems(source);
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
                    this.source.CollectionChanged -= this.OnTrackedCollectionChanged;
                }

                this.source = newSource;
                if (newSource == null)
                {
                    foreach (var kvp in this.map)
                    {
                        kvp.Value.TrackedPropertyChanged -= this.OnTrackedItemChanged;
                        kvp.Value.Dispose();
                    }

                    this.map.Clear();
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
                        collection.CollectionChanged -= this.OnTrackedCollectionChanged;
                        this.source = null;
                    }

                    foreach (var kvp in this.map)
                    {
                        kvp.Value.TrackedPropertyChanged -= this.OnTrackedItemChanged;
                        kvp.Value.Dispose();
                    }

                    this.map.Clear();
                }
            }

            base.Dispose(disposing);
        }

        private void OnTrackedItemChanged(IPropertyTracker tracker, object sender, PropertyChangedEventArgs e, SourceAndValue<INotifyPropertyChanged, TProperty> sourceAndValue)
        {
            this.OnTrackedItemChanged(
                ((PropertyPathTracker<TItem, TProperty>)tracker.PathTracker).Source,
                sender,
                e,
                sourceAndValue);
        }

        private void OnTrackedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
                    case NotifyCollectionChangedAction.Replace:
                        this.AddItems(e.NewItems);
                        this.RemoveItems(e.OldItems);
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

        private void AddItems(IEnumerable items)
        {
            foreach (TItem item in items)
            {
                if (item == null ||
                    this.map.ContainsKey(item))
                {
                    continue;
                }

                var tracker = this.path.CreateTracker(item);
                //// Signaling initial before subscribing here to get the events in correct order
                //// This can't be made entirely thread safe as an event can be raised on source between signal initial & subscribe.
                this.SignalInitial(tracker);
                tracker.TrackedPropertyChanged += this.OnTrackedItemChanged;
                this.map.Add(item, tracker);
            }
        }

        private void RemoveItems(IEnumerable items)
        {
            var set = IdentitySet.Borrow<TItem>();
            foreach (var item in items)
            {
                set.Add((TItem)item);
            }

            set.ExceptWith(this.source ?? Enumerable.Empty<TItem>());
            foreach (var item in set)
            {
                if (item == null)
                {
                    continue;
                }

                var tracker = this.map[item];
                tracker.TrackedPropertyChanged -= this.OnTrackedItemChanged;
                tracker.Dispose();
                this.map.Remove(item);
            }

            set.Clear();
            IdentitySet.Return(set);
        }

        private void SignalInitial(PropertyPathTracker<TItem, TProperty> tracker)
        {
            if (!this.HasSubscribers)
            {
                return;
            }

            var sourceAndValue = tracker.SourceAndValue();
            this.OnTrackedItemChanged(
                tracker.Source,
                sourceAndValue.Source,
                CachedEventArgs.GetOrCreatePropertyChangedEventArgs(string.Empty),
                sourceAndValue);
        }
    }
}
