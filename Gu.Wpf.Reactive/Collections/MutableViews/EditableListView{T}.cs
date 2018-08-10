// ReSharper disable MemberCanBePrivate.Global
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Disposables;

    using Gu.Reactive;

    /// <summary>
    /// Decorate an <see cref="IObservableCollection{T}"/> with <see cref="IList"/>.
    /// </summary>
    public class EditableListView<T> : Collection<T>, IObservableCollection<T>, IDisposable
    {
        private readonly bool leaveOpen;
        private readonly IDisposable subscriptions;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableListView{T}"/> class.
        /// </summary>
        /// <param name="list">The collection to decorate.</param>
        /// <param name="leaveOpen">True means that <paramref name="list"/> is not disposed when this instance is diposed.</param>
        public EditableListView(IObservableCollection<T> list, bool leaveOpen)
            : base(list)
        {
            this.leaveOpen = leaveOpen;
            this.subscriptions = new CompositeDisposable(2)
                                     {
                                         list.ObservePropertyChangedSlim()
                                               .Subscribe(this.OnPropertyChanged),
                                         list.ObserveCollectionChangedSlim(signalInitial: false)
                                               .Subscribe(this.OnCollectionChanged),
                                     };
        }

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public void Move(int oldIndex, int newIndex)
        {
            var item = this[oldIndex];
            this.RemoveItem(oldIndex);
            this.InsertItem(newIndex, item);
            this.OnPropertyChanged(CachedEventArgs.IndexerPropertyChanged);
            this.OnCollectionChanged(Diff.CreateMoveEventArgs(item, newIndex, oldIndex));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Disposes of a <see cref="EditableListView{T}"/>.
        /// </summary>
        /// <remarks>
        /// Called from Dispose() with disposing=true.
        /// Guidelines:
        /// 1. We may be called more than once: do nothing after the first call.
        /// 2. Avoid throwing exceptions if disposing is false, i.e. if we're being finalized.
        /// </remarks>
        /// <param name="disposing">True if called from Dispose(), false if called from the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.subscriptions.Dispose();
                if (!this.leaveOpen)
                {
                    (this.Items as IDisposable)?.Dispose();
                }
            }
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

        /// <summary>
        /// Raise PropertyChanged event to any listeners.
        /// Properties/methods modifying this <see cref="EditableListView{T}"/> will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                (this.Items as IRefreshAble)?.Refresh();
                handler(this, e);
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
    }
}
