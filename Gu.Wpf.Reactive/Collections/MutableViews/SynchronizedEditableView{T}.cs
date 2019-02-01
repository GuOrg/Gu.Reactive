namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using Gu.Reactive;
    using Gu.Reactive.Internals;

    /// <summary>
    /// A synchronized view of a collection that supports two way bindings.
    /// </summary>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {this.Count}")]
    [Serializable]
    public abstract class SynchronizedEditableView<T> : Collection<T>, IRefreshAble, IDisposable, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private readonly bool leaveOpen;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedEditableView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="leaveOpen">True means that <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="startEmpty">If source should be added to the <see cref="Tracker"/> by the constructor.</param>
        protected SynchronizedEditableView(IList<T> source, bool leaveOpen, bool startEmpty)
            : this(source, source, leaveOpen, startEmpty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedEditableView{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection.</param>
        /// <param name="sourceItems">The source items, this can be a filtered view of <paramref name="source"/>.</param>
        /// <param name="leaveOpen">True means that <paramref name="source"/> is not disposed when this instance is disposed.</param>
        /// <param name="startEmpty">If source should be added to the <see cref="Tracker"/> by the constructor.</param>
        protected SynchronizedEditableView(IList<T> source, IEnumerable<T> sourceItems, bool leaveOpen, bool startEmpty)
            : this(source, new CollectionSynchronizer<T>(startEmpty ? Enumerable.Empty<T>() : sourceItems), leaveOpen)
        {
        }

        private SynchronizedEditableView(IList<T> source, CollectionSynchronizer<T> tracker, bool leaveOpen)
            : base(tracker)
        {
            Ensure.NotNull(source, nameof(source));
            this.leaveOpen = leaveOpen;
            this.Source = source;
            this.Tracker = tracker;
        }

        /// <inheritdoc/>
        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        [field: NonSerialized]
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// The source collection.
        /// </summary>
        protected IList<T> Source { get; }

        /// <summary>
        /// The <see cref="CollectionSynchronizer{T}"/> that keeps this in sync with <see cref="Source"/>.
        /// </summary>
        protected CollectionSynchronizer<T> Tracker { get; }

        /// <summary>
        /// Returns true if there are any subscribers to the <see cref="PropertyChanged"/> or <see cref="CollectionChanged"/> events.
        /// </summary>
        protected bool HasListeners => this.PropertyChanged != null || this.CollectionChanged != null;

        /// <summary>
        /// The event we updated the source with.
        /// </summary>
        protected NotifyCollectionChangedEventArgs UpdatedSourceWith { get; private set; }

        /// <summary>
        /// Move item at oldIndex to newIndex.
        /// </summary>
        public void Move(int oldIndex, int newIndex)
        {
            this.MoveItem(oldIndex, newIndex);
        }

        /// <inheritdoc/>
        public virtual void Refresh()
        {
            (this.Source as IRefreshAble)?.Refresh();
            if (this.HasListeners)
            {
                this.Tracker.Reset(this.Source, this.OnPropertyChanged, this.OnCollectionChanged);
            }
            else
            {
                this.Tracker.Reset(this.Source);
            }
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly, bug in CA1063
        /// <inheritdoc/>
        public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Pass null as scheduler here, change came from the UI-thread.
        /// </summary>
        protected virtual void Notify(NotifyCollectionChangedEventArgs change)
        {
            if (change == null || !this.HasListeners)
            {
                return;
            }

            switch (change.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    this.OnPropertyChanged(CachedEventArgs.CountPropertyChanged);
                    this.OnPropertyChanged(CachedEventArgs.IndexerPropertyChanged);
                    this.OnCollectionChanged(change);
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    this.OnPropertyChanged(CachedEventArgs.IndexerPropertyChanged);
                    this.OnCollectionChanged(change);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.OnPropertyChanged(CachedEventArgs.CountPropertyChanged);
                    this.OnPropertyChanged(CachedEventArgs.IndexerPropertyChanged);
                    this.OnCollectionChanged(change);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(change));
            }
        }

        /// <summary>
        /// Refreshes the view. May be deferred if there is a buffer time.
        /// </summary>
        protected virtual void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            if (changes.Count == 0)
            {
                return;
            }

            if (this.HasListeners)
            {
                this.Tracker.Refresh(this.Source, changes, this.OnPropertyChanged, this.OnCollectionChanged);
            }
            else
            {
                this.Tracker.Refresh(this.Source, changes);
            }
        }

        /// <summary>
        /// Return true if the change came from the source, False if we updated the source to trigger the event.
        /// </summary>
        protected virtual bool IsSourceChange(NotifyCollectionChangedEventArgs e)
        {
            return this.UpdatedSourceWith?.Action != e.Action;
        }

        /// <inheritdoc/>
        protected override void ClearItems()
        {
            base.ClearItems();
            var change = CachedEventArgs.NotifyCollectionReset;
            this.UpdatedSourceWith = change;
            this.Notify(change);
            this.Source.Clear();
            this.UpdatedSourceWith = null;
        }

        /// <inheritdoc/>
        protected override void InsertItem(int index, T item)
        {
            var sourceIndex = index == this.Count
                ? this.Source.Count
                : this.SourceIndex(index);
            base.InsertItem(index, item);
            var change = Diff.CreateAddEventArgs(item, index);
            this.UpdatedSourceWith = change;
            this.Notify(change);
            this.Source.Insert(sourceIndex, item);
            this.UpdatedSourceWith = null;
        }

        /// <inheritdoc/>
        protected override void RemoveItem(int index)
        {
            var item = this[index];
            var sourceIndex = this.SourceIndex(index);
            base.RemoveItem(index);
            var change = Diff.CreateRemoveEventArgs(item, index);
            this.UpdatedSourceWith = change;
            this.Notify(change);
            if (sourceIndex >= 0)
            {
                this.Source.RemoveAt(sourceIndex);
            }

            this.UpdatedSourceWith = null;
        }

        /// <inheritdoc/>
        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            var sourceIndex = this.SourceIndex(index);
            base.SetItem(index, item);
            var change = Diff.CreateReplaceEventArgs(item, oldItem, index);
            this.UpdatedSourceWith = change;
            this.Notify(change);
            if (sourceIndex >= 0)
            {
                this.Source[sourceIndex] = item;
            }

            this.UpdatedSourceWith = null;
        }

        /// <summary>
        /// Move item at oldIndex to newIndex.
        /// </summary>
        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            var sourceOldIndex = this.SourceIndex(oldIndex);
            var sourceNewIndex = this.SourceIndex(newIndex);
            var item = this[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, item);
            var change = Diff.CreateMoveEventArgs(item, newIndex, oldIndex);
            this.UpdatedSourceWith = change;
            this.Notify(change);
            if (sourceOldIndex >= 0 &&
                sourceNewIndex >= 0)
            {
                if (this.Source is ObservableCollection<T> s1)
                {
                    s1.Move(sourceOldIndex, sourceNewIndex);
                }
                else if (this.Source is IObservableCollection<T> s2)
                {
                    s2.Move(sourceOldIndex, sourceNewIndex);
                }
                else
                {
                    throw new NotSupportedException($"Expected source to be ObservableCollection<T> or IObservableCollection<T>, was {this.Source.GetType().PrettyName()}");
                }
            }

            this.UpdatedSourceWith = null;
        }

        /// <summary>
        /// Get the corresponding index in <see cref="Source"/>.
        /// </summary>
        protected virtual int SourceIndex(int index)
        {
            var equals = typeof(T).IsValueType
                ? (Func<T, T, bool>)EqualityComparer<T>.Default.Equals
                : (x, y) => ReferenceEquals(x, y);
            var sourceIndex = 0;
            for (var i = 0; i <= index; i++)
            {
                while (!equals(this.Source[sourceIndex], this[i]))
                {
                    sourceIndex++;
                    if (sourceIndex >= this.Source.Count)
                    {
                        return -1;
                    }
                }

                if (i == index)
                {
                    return sourceIndex;
                }

                sourceIndex++;
            }

            return -1;
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                if (!this.leaveOpen)
                {
#pragma warning disable IDISP007 // Don't dispose injected.
                    (this.Source as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
                }
            }
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Calls <see cref="OnPropertyChanged(PropertyChangedEventArgs)"/>.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="SynchronizedEditableView{T}"/> will raise
        /// a property changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raise CollectionChanged event to any listeners.
        /// Properties/methods modifying this <see cref="EditableListView{T}"/> will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }
    }
}
