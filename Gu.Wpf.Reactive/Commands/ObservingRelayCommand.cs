// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Linq;

    using Gu.Reactive.Internals;

    /// <summary>
    /// A command that does not use the CommandParameter
    /// Signals CanExecuteChanged when observable signals
    /// </summary>
    public class ObservingRelayCommand : ManualRelayCommand, IDisposable
    {
        private readonly IDisposable subscription;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservingRelayCommand"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        /// <param name="criteria">The criteria for CanExecute</param>
        /// <param name="observable">The observable notifying about update of CanExecute</param>
        public ObservingRelayCommand(
            Action action,
            Func<bool> criteria,
            params IObservable<object>[] observable)
            : base(action, criteria)
        {
            Ensure.NotNullOrEmpty(observable, nameof(observable));
            this.subscription = observable.Merge()
                                      .Subscribe(_ => this.RaiseCanExecuteChanged());
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <inheritdoc/>
        protected override bool InternalCanExecute(object parameter)
        {
            this.VerifyDisposed();
            return base.InternalCanExecute(parameter);
        }

        /// <inheritdoc/>
        protected override void InternalExecute(object parameter)
        {
            this.VerifyDisposed();
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
        protected void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}