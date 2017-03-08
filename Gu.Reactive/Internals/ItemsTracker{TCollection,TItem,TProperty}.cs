namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    internal sealed class ItemsTracker<TCollection, TItem, TProperty> : IDisposable
        where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
        where TItem : class, INotifyPropertyChanged
    {
        private readonly NotifyingPath<TItem, TProperty> propertyPath;
        private readonly Dictionary<TItem, PropertyPathTracker> map = new Dictionary<TItem, PropertyPathTracker>(ObjectIdentityComparer<TItem>.Default);
        private readonly HashSet<TItem> set = new HashSet<TItem>(ObjectIdentityComparer<TItem>.Default);
        private readonly object gate = new object();

        private TCollection source;
        private bool disposed;

        public ItemsTracker(TCollection source, NotifyingPath<TItem, TProperty> propertyPath)
        {
            this.propertyPath = propertyPath;
            if (source != null)
            {
                this.AddItems(source);
                this.UpdateSource(source);
            }
        }

        internal event TrackedPropertyChangedEventHandler TrackedItemChanged;

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }

            lock (this.gate)
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
                    kvp.Value.Last.TrackedPropertyChanged -= this.OnTrackedItemChanged;
                    kvp.Value.Dispose();
                }

                this.map.Clear();
                this.set.Clear();
            }
        }

        internal void UpdateSource(TCollection newSource)
        {
            if (this.disposed ||
                ReferenceEquals(this.source, newSource))
            {
                return;
            }

            lock (this.gate)
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
                        kvp.Value.Last.TrackedPropertyChanged -= this.OnTrackedItemChanged;
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

        private void OnTrackedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.disposed)
            {
                return;
            }

            lock (this.gate)
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
                if (item == null || this.map.ContainsKey(item))
                {
                    continue;
                }

                var tracker = PropertyPathTracker.Create(item, this.propertyPath);
                //// Signaling initial before subscrinbing here to get the events in correct order
                //// This can't be made entirely thread safe as an event can be raised on source bewteen signal initial & subscribe.
                this.SignalInitial(tracker);
                tracker.Last.TrackedPropertyChanged += this.OnTrackedItemChanged;
                this.map.Add(item, tracker);
            }
        }

        private void RemoveItems(IEnumerable<TItem> items)
        {
            this.set.Clear();
            this.set.UnionWith(items);
            this.set.ExceptWith(this.source ?? Enumerable.Empty<TItem>());
            foreach (var item in this.set)
            {
                if (item == null)
                {
                    continue;
                }

                this.map[item].Last.TrackedPropertyChanged -= this.OnTrackedItemChanged;
                this.map[item].Dispose();
                this.map.Remove(item);
            }

            this.set.Clear();
        }

        private void OnTrackedItemChanged(PathPropertyTracker tracker, object sender, PropertyChangedEventArgs e, SourceAndValue<object> sourceAndValue)
        {
            this.TrackedItemChanged?.Invoke(tracker, sender, e, sourceAndValue);
        }

        private void SignalInitial(PropertyPathTracker tracker)
        {
            var handler = this.TrackedItemChanged;
            if (handler != null)
            {
                var sourceAndValue = tracker.SourceAndValue();
                handler.Invoke(tracker.First, sourceAndValue.Source, CachedEventArgs.GetOrCreatePropertyChangedEventArgs(string.Empty), sourceAndValue);
            }
        }
    }
}