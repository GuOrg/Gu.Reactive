namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Helper for synchronizing two collections and notifying about diffs.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    [Serializable]
    public class CollectionSynchronizer<T> : Collection<T>
    {
        private readonly List<T> temp = new List<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionSynchronizer{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        public CollectionSynchronizer(IEnumerable<T> source)
        {
            this.Reset(source ?? Enumerable.Empty<T>());
        }

        /// <summary>
        /// Set this to <paramref name="updated"/> and notify about changes.
        /// </summary>
        /// <param name="updated">The updated collection.</param>
        public void Reset(IEnumerable<T> updated)
        {
            this.Refresh(updated, Array.Empty<NotifyCollectionChangedEventArgs>(), null, null);
        }

        /// <summary>
        /// Set to <paramref name="updated"/> and notify about changes.
        /// </summary>
        /// <param name="updated">The updated collection.</param>
        /// <param name="propertyChanged">The <see cref="Action{PropertyChangedEventArgs}"/> to notify on.</param>
        /// <param name="collectionChanged">The <see cref="Action{NotifyCollectionChangedEventArgs}"/> to notify on.</param>
        public void Reset(IEnumerable<T> updated, Action<PropertyChangedEventArgs> propertyChanged, Action<NotifyCollectionChangedEventArgs> collectionChanged)
        {
            this.Refresh(updated, Array.Empty<NotifyCollectionChangedEventArgs>(), propertyChanged, collectionChanged);
        }

        /// <summary>
        /// Set to <paramref name="updated"/> and notify about changes.
        /// </summary>
        /// <param name="updated">The updated collection.</param>
        /// <param name="collectionChanges">The captured collection change args.</param>
        /// <param name="propertyChanged">The <see cref="Action{PropertyChangedEventArgs}"/> to notify on.</param>
        /// <param name="collectionChanged">The <see cref="Action{NotifyCollectionChangedEventArgs}"/> to notify on.</param>
        public void Refresh(IEnumerable<T> updated, IReadOnlyList<NotifyCollectionChangedEventArgs>? collectionChanges = null, Action<PropertyChangedEventArgs>? propertyChanged = null, Action<NotifyCollectionChangedEventArgs>? collectionChanged = null)
        {
            lock (this.Items)
            {
                var change = this.Update(updated, collectionChanges, propertyChanged, collectionChanged);
                if (change is null)
                {
                    return;
                }

                switch (change.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                        propertyChanged?.Invoke(CachedEventArgs.CountPropertyChanged);
                        propertyChanged?.Invoke(CachedEventArgs.IndexerPropertyChanged);
                        collectionChanged?.Invoke(change);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Move:
                        propertyChanged?.Invoke(CachedEventArgs.IndexerPropertyChanged);
                        collectionChanged?.Invoke(change);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        propertyChanged?.Invoke(CachedEventArgs.CountPropertyChanged);
                        propertyChanged?.Invoke(CachedEventArgs.IndexerPropertyChanged);
                        collectionChanged?.Invoke(change);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(updated), change.Action, "Should never get here");
                }
            }
        }

        /// <summary>
        /// Lock the collection and get a copy.
        /// </summary>
        /// <returns>A copy of this instance.</returns>
        public IReadOnlyList<T> Snapshot()
        {
            lock (this.Items)
            {
                return new List<T>(this.Items);
            }
        }

        private NotifyCollectionChangedEventArgs? Update(IEnumerable<T> updated, IReadOnlyList<NotifyCollectionChangedEventArgs>? collectionChanges, Action<PropertyChangedEventArgs>? propertyChanged, Action<NotifyCollectionChangedEventArgs>? collectionChanged)
        {
            var retry = 0;
            while (true)
            {
                this.temp.Clear();
                try
                {
                    this.temp.AddRange(updated);
                    break;
                }
                catch (InvalidOperationException e)
                    when (e.Message == Exceptions.CollectionWasModified.Message &&
                          retry < 5)
                {
                    retry++;
                }
                catch (ArgumentException e)
                    when (e.Message == Exceptions.DestinationNotLongEnough.Message &&
                          retry < 5)
                {
                    retry++;
                }
            }

            if (propertyChanged != null || collectionChanged != null)
            {
                var change = Diff.CollectionChange(this, this.temp, collectionChanges);
                this.Items.Clear();
                ((List<T>)this.Items).AddRange(this.temp);
                this.temp.Clear();
                return change;
            }

            this.Items.Clear();
            ((List<T>)this.Items).AddRange(this.temp);
            this.temp.Clear();
            return null;
        }
    }
}
