// ReSharper disable MemberCanBePrivate.Global
namespace Gu.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reactive.Disposables;

    /// <summary>
    /// Decorate an <see cref="IObservableCollection{T}"/> with <see cref="IList"/>
    /// </summary>
    public class EditableListView<T> : Collection<T>, IObservableCollection<T>, IDisposable
    {
        private readonly IDisposable subscriptions;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditableListView{T}"/> class.
        /// </summary>
        public EditableListView(IObservableCollection<T> list)
            : base(list)
        {
            this.subscriptions = new CompositeDisposable(2)
                                     {
                                         list.ObservePropertyChangedSlim()
                                               .Subscribe(this.OnPropertyChanged),
                                         list.ObserveCollectionChangedSlim(false)
                                               .Subscribe(this.OnCollectionChanged)
                                     };
        }

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

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
        protected void ThwrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}