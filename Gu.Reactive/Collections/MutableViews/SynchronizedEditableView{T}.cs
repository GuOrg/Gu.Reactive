namespace Gu.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Reactive.Internals;

    /// <summary>
    /// A synchronized view of a collection that supports two way bindings.
    /// </summary>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    [DebuggerDisplay("Count = {this.Count}")]
    [Serializable]
    [Obsolete("Candidate for removal, broken. Prefer the read only version.")]
    public abstract class SynchronizedEditableView<T> : Collection<T>, IUpdater, IRefreshAble, INotifyPropertyChanged, INotifyCollectionChanged
    {
        private object isUpdatingSourceItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedEditableView{T}"/> class.
        /// </summary>
        protected SynchronizedEditableView(IList<T> source, bool startEmpty)
            : this(source, source, startEmpty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedEditableView{T}"/> class.
        /// </summary>
        protected SynchronizedEditableView(IList<T> source, IEnumerable<T> sourceItems, bool startEmpty)
            : this(source, new CollectionSynchronizer<T>(startEmpty ? Enumerable.Empty<T>() : sourceItems))
        {
        }

        private SynchronizedEditableView(IList<T> source, CollectionSynchronizer<T> tracker)
            : base(tracker)
        {
            Ensure.NotNull(source, nameof(source));
            this.Source = source;
            this.Tracker = tracker;
        }

        /// <inheritdoc/>
        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        [field: NonSerialized]
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc/>
        object IUpdater.CurrentlyUpdatingSourceItem => this.isUpdatingSourceItem;

        /// <summary>
        /// The source collection.
        /// </summary>
        protected IList<T> Source { get; }

        /// <summary>
        /// The <see cref="CollectionSynchronizer{T}"/> that keeps this in sync with <see cref="Source"/>
        /// </summary>
        protected CollectionSynchronizer<T> Tracker { get; }

        /// <summary>
        /// Returns true if there are any subscribers to the <see cref="PropertyChanged"/> or <see cref="CollectionChanged"/> events.
        /// </summary>
        protected bool HasListeners => this.PropertyChanged != null || this.CollectionChanged != null;

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

        /// <summary>
        /// Pass null as scheduler here, change came from the ui thread.
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
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Refreshes the view. May be deferred if there is a buffer time.
        /// </summary>
        protected virtual void Refresh(IReadOnlyList<NotifyCollectionChangedEventArgs> changes)
        {
            if (this.HasListeners)
            {
                this.Tracker.Refresh(this.Source, changes, this.OnPropertyChanged, this.OnCollectionChanged);
            }
            else
            {
                this.Tracker.Refresh(this.Source, changes);
            }
        }

        /// <inheritdoc/>
        protected override void ClearItems()
        {
            base.ClearItems();
            this.Notify(CachedEventArgs.NotifyCollectionReset);
            this.Source.Clear();
        }

        /// <inheritdoc/>
        protected override void InsertItem(int index, T item)
        {
            this.isUpdatingSourceItem = item;
            base.InsertItem(index, item);
            this.Notify(Diff.CreateAddEventArgs(item, index));
            this.Source.Insert(index, item);
            this.isUpdatingSourceItem = null;
        }

        /// <inheritdoc/>
        protected override void RemoveItem(int index)
        {
            var item = this[index];
            this.isUpdatingSourceItem = item;
            base.RemoveItem(index);
            this.Notify(Diff.CreateRemoveEventArgs(item, index));
            this.Source.Remove(item);
            this.isUpdatingSourceItem = null;
        }

        /// <inheritdoc/>
        protected override void SetItem(int index, T item)
        {
            this.isUpdatingSourceItem = item;
            var oldItem = this[index];
            base.SetItem(index, item);
            this.Notify(Diff.CreateReplaceEventArgs(item, oldItem, index));
            var i = this.Source.IndexOf(oldItem);
            if (i >= 0)
            {
                this.Source[i] = item;
            }

            this.isUpdatingSourceItem = null;
        }

        /// <summary>
        /// Calls <see cref="OnPropertyChanged(PropertyChangedEventArgs)"/>
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
