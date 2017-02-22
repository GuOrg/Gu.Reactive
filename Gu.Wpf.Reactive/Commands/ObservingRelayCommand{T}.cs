// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Linq;

    using Gu.Reactive.Internals;
    using Gu.Reactive.Internals.Ensure;

    /// <summary>
    /// A command with CommandParameter of type <typeparamref name="T"/>
    /// Signals CanExecuteChanged when observable signals
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    public class ObservingRelayCommand<T> : ManualRelayCommand<T>, IDisposable
    {
        private readonly IDisposable subscription;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservingRelayCommand{T}"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        /// <param name="criteria">The criteria for CanExecute</param>
        /// <param name="observable">The observable notifying about update of CanExecute</param>
        public ObservingRelayCommand(
            Action<T> action,
            Func<T, bool> criteria,
            params IObservable<object>[] observable)
            : base(action, criteria)
        {
            Ensure.NotNullOrEmpty(observable, nameof(observable));
            this.subscription = observable.Merge()
                                      .Subscribe(x => this.RaiseCanExecuteChanged());
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <inheritdoc/>
        protected override bool InternalCanExecute(T parameter)
        {
            this.ThrowIfDisposed();
            return base.InternalCanExecute(parameter);
        }

        /// <inheritdoc/>
        protected override void InternalExecute(T parameter)
        {
            this.ThrowIfDisposed();
            base.InternalExecute(parameter);
        }

        /// <summary>
        /// Disposes of a <see cref="ObservingRelayCommand"/>.
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
                this.subscription.Dispose();
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