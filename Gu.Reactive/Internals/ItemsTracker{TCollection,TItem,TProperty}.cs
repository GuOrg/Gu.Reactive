namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reactive;

    internal sealed class ItemsTracker<TCollection, TItem, TProperty> : IDisposable
        where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
        where TItem : class, INotifyPropertyChanged
    {
        private readonly NotifyingPath<TItem, TProperty> propertyPath;
        private readonly IObserver<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> observer;
        private readonly Dictionary<TItem, IDisposable> map = new Dictionary<TItem, IDisposable>(ObjectIdentityComparer<TItem>.Default);
        private readonly HashSet<TItem> set = new HashSet<TItem>(ObjectIdentityComparer<TItem>.Default);
        private readonly object gate = new object();

        private TCollection source;
        private bool disposed;

        public ItemsTracker(
            TCollection source,
            NotifyingPath<TItem, TProperty> propertyPath,
            IObserver<EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>> observer,
            bool signalInitial = true)
        {
            this.propertyPath = propertyPath;
            this.observer = observer;
            if (source != null)
            {
                this.AddItems(source, signalInitial);
                this.UpdateSource(source);
            }
        }

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
                        this.AddItems(e.NewItems.OfType<TItem>(), true);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        this.RemoveItems(e.OldItems.OfType<TItem>());
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        this.AddItems(e.NewItems.OfType<TItem>(), true);
                        this.RemoveItems(e.OldItems.OfType<TItem>());
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        this.RemoveItems(this.map.Keys);
                        this.AddItems(this.source, true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void AddItems(IEnumerable<TItem> items, bool signalInitial)
        {
            foreach (var item in items)
            {
                if (item == null || this.map.ContainsKey(item))
                {
                    continue;
                }

                var subscription = item.ObservePropertyChangedWithValue(this.propertyPath, signalInitial)
                                       .Subscribe(x => this.observer.OnNext(new EventPattern<ItemPropertyChangedEventArgs<TItem, TProperty>>(x.Sender, new ItemPropertyChangedEventArgs<TItem, TProperty>(item, x))));
                this.map.Add(item, subscription);
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

                this.map[item].Dispose();
                this.map.Remove(item);
            }

            this.set.Clear();
        }
    }
}