namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Gu.Reactive.Internals;

    /// <summary>
    /// Helper for synchronizing two coillections and notifying about diffs.
    /// </summary>
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public class CollectionSynchronizer<T> : Collection<T>
    {
        private readonly List<T> temp = new List<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionSynchronizer{T}"/> class.
        /// </summary>
        public CollectionSynchronizer(IEnumerable<T> source)
        {
            this.Reset(source ?? Enumerable.Empty<T>());
        }

        /// <summary>
        /// Returns the index of the last occurrence of a given value in a range of
        /// this list. The list is searched backwards, starting at the end 
        /// and ending at the first element in the list. The elements of the list 
        /// are compared to the given value using the Object.Equals method.
        ///
        /// This method uses the Array.LastIndexOf method to perform the
        /// search.
        /// </summary>
        public int LastIndexOf(T value)
        {
            return ((List<T>)this.Items).LastIndexOf(value);
        }

        /// <summary>
        /// Set <see cref="Current"/> to <paramref name="updated"/> and notify about changes.
        /// </summary>
        /// <param name="updated">The updated collection.</param>
        public void Reset(IEnumerable<T> updated)
        {
            this.Refresh(updated, CachedEventArgs.EmptyArgs, null, null);
        }

        /// <summary>
        /// Set <see cref="Current"/> to <paramref name="updated"/> and notify about changes.
        /// </summary>
        /// <param name="updated">The updated collection.</param>
        /// <param name="propertyChanged">The <see cref="Action{PropertyChangedEventArgs}"/> to notify on.</param>
        /// <param name="collectionChanged">The <see cref="Action{NotifyCollectionChangedEventArgs}"/> to notify on.</param>
        public void Reset(IEnumerable<T> updated, Action<PropertyChangedEventArgs> propertyChanged, Action<NotifyCollectionChangedEventArgs> collectionChanged)
        {
            this.Refresh(updated, CachedEventArgs.EmptyArgs, propertyChanged, collectionChanged);
        }

        /// <summary>
        /// Set <see cref="Current"/> to <paramref name="updated"/> and notify about changes.
        /// </summary>
        /// <param name="updated">The updated collection.</param>
        /// <param name="collectionChanges">The captured collection change args.</param>
        /// <param name="propertyChanged">The <see cref="Action{PropertyChangedEventArgs}"/> to notify on.</param>
        /// <param name="collectionChanged">The <see cref="Action{NotifyCollectionChangedEventArgs}"/> to notify on.</param>
        public void Refresh(IEnumerable<T> updated, IReadOnlyList<NotifyCollectionChangedEventArgs> collectionChanges = null, Action<PropertyChangedEventArgs> propertyChanged = null, Action<NotifyCollectionChangedEventArgs> collectionChanged = null)
        {
            lock (this.Items)
            {
                var change = this.Update(updated, collectionChanges, propertyChanged, collectionChanged);
                if (change == null)
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
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private NotifyCollectionChangedEventArgs Update(IEnumerable<T> updated, IReadOnlyList<NotifyCollectionChangedEventArgs> collectionChanges, Action<PropertyChangedEventArgs> propertyChanged, Action<NotifyCollectionChangedEventArgs> collectionChanged)
        {
            const int retries = 5;
            for (var i = 1; i <= retries; i++)
            {
                this.temp.Clear();
                try
                {
                    this.temp.AddRange(updated);
                    break;
                }
                catch (InvalidOperationException e) when (e.Message == Exceptions.CollectionWasModified.Message)
                {
                    if (i >= retries)
                    {
                        throw;
                    }
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

        private static class Exceptions
        {
            private static InvalidOperationException collectionWasModified;

            public static InvalidOperationException CollectionWasModified => collectionWasModified ?? (collectionWasModified = Create());

            private static InvalidOperationException Create()
            {
                var ints = new List<int>(1);
                try
                {
                    using (var enumerator = ints.GetEnumerator())
                    {
                        // this increments version of the list.
                        ints.Clear();

                        // this throws collection was modified.
                        enumerator.MoveNext();
                    }
                }
                catch (InvalidOperationException e)
                {
                    return e;
                }

                throw new NotImplementedException("Should never get here.");
            }
        }
    }
}