namespace Gu.Reactive.Internals
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal sealed class NestedItemsTrackerSlim<TCollection, TItem, TProperty> : ItemsTrackerSlim
        where TCollection : class, IEnumerable<TItem>, INotifyCollectionChanged
        where TItem : class?, INotifyPropertyChanged?
    {
        private readonly object gate = new object();
        private readonly TCollection source;
        private readonly NotifyingPath<TItem, TProperty> path;
#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
        private readonly IdentityMap<TItem, PropertyPathTracker<TItem, TProperty>> map = IdentityMap.Borrow<TItem, PropertyPathTracker<TItem, TProperty>>();
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.

        internal NestedItemsTrackerSlim(TCollection source, NotifyingPath<TItem, TProperty> path)
        {
            this.source = source;
            this.path = path;
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

                    foreach (var tracker in this.map.Values)
                    {
                        tracker.TrackedPropertyChanged -= this.OnItemPropertyChanged;
                        tracker.Dispose();
                    }

#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
                    IdentityMap.Return(this.map);
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
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
                case NotifyCollectionChangedAction.Replace
                    when e.OldItems.TrySingle(out var oldItem) &&
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
                    this.UpdateSubscriptions(this.map.Keys, this.source);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(e), e, "Unknown NotifyCollectionChangedAction.");
            }
        }

        private void UpdateSubscriptions(IEnumerable? oldItems, IEnumerable newItems)
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

#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
                var set = IdentitySet.Borrow<TItem>();
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
                set.UnionWith(this.map.Keys);
                if (newItems != null)
                {
#pragma warning disable CS8606 // Possible null reference assignment to iteration variable
                    foreach (TItem item in newItems)
#pragma warning restore CS8606 // Possible null reference assignment to iteration variable
                    {
                        if (!(item is null) &&
                            set.Add(item))
                        {
                            var tracker = this.path.CreateTracker(item);
                            tracker.TrackedPropertyChanged += this.OnItemPropertyChanged;
                            this.map.Add(item, tracker);
                        }
                    }
                }

                if (oldItems != null)
                {
                    set.IntersectWith(this.source.NotNull());
#pragma warning disable CS8606 // Possible null reference assignment to iteration variable
                    foreach (TItem item in oldItems)
#pragma warning restore CS8606 // Possible null reference assignment to iteration variable
                    {
                        if (!(item is null) &&
                            !set.Contains(item))
                        {
                            var tracker = this.map[item];
                            tracker.TrackedPropertyChanged -= this.OnItemPropertyChanged;
                            tracker.Dispose();
                            this.map.Remove(item);
                        }
                    }
                }

#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
                IdentitySet.Return(set);
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
            }
        }

        private void OnItemPropertyChanged(IPropertyTracker tracker, object sender, PropertyChangedEventArgs e, SourceAndValue<INotifyPropertyChanged?, TProperty> sourceAndValue)
        {
            this.OnItemPropertyChanged(e);
        }
    }
}
