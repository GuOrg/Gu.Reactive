namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    internal sealed class NestedItemsTracker<TCollection, TItem, TProperty> : ItemsTracker<TCollection, TItem, TProperty>
        where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
        where TItem : class, INotifyPropertyChanged
    {
        private static readonly ConcurrentQueue<HashSet<TItem>> SetPool = new ConcurrentQueue<HashSet<TItem>>();

        private readonly NotifyingPath<TItem, TProperty> path;
        private readonly Dictionary<TItem, PropertyPathTracker<TItem, TProperty>> map = new Dictionary<TItem, PropertyPathTracker<TItem, TProperty>>(ObjectIdentityComparer<TItem>.Default);

        private TCollection source;

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

        private void OnTrackedItemChanged(IPathPropertyTracker tracker, object sender, PropertyChangedEventArgs e, SourceAndValue<INotifyPropertyChanged, TProperty> sourceAndValue)
        {
            this.OnTrackedItemChanged(
                ((PropertyPathTracker<TItem, TProperty>)tracker.PathTracker).Source,
                sender,
                e,
                sourceAndValue);
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
                        this.RemoveItems(this.map.Keys);
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
                    this.map.ContainsKey(item))
                {
                    continue;
                }

                var tracker = this.path.CreateTracker(item);
                //// Signaling initial before subscrinbing here to get the events in correct order
                //// This can't be made entirely thread safe as an event can be raised on source bewteen signal initial & subscribe.
                this.SignalInitial(tracker);
                tracker.TrackedPropertyChanged += this.OnTrackedItemChanged;
                this.map.Add(item, tracker);
            }
        }

        private void RemoveItems(IEnumerable<TItem> items)
        {
            var set = SetPool.GetOrCreate(() => new HashSet<TItem>(ObjectIdentityComparer<TItem>.Default));
            set.UnionWith(items);
            set.ExceptWith(this.source ?? Enumerable.Empty<TItem>());
            foreach (var item in set)
            {
                if (item == null)
                {
                    continue;
                }

                this.map[item].TrackedPropertyChanged -= this.OnTrackedItemChanged;
                this.map[item].Dispose();
                this.map.Remove(item);
            }

            set.Clear();
            SetPool.Enqueue(set);
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