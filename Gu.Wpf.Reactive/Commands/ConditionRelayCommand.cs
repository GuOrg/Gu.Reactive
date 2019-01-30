namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive;

    /// <summary>
    /// A command that does not use the CommandParameter.
    /// </summary>
    public class ConditionRelayCommand : ManualRelayCommand, IConditionRelayCommand
    {
        private readonly IDisposable subscription;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionRelayCommand"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        /// <param name="condition">The criteria by CanExecute.</param>
        public ConditionRelayCommand(Action action, ICondition condition)
            : base(action, () => condition.IsSatisfied == true)
        {
            this.Condition = condition;
            this.subscription = condition.ObserveIsSatisfiedChanged()
                                         .Subscribe(_ => this.RaiseCanExecuteChanged());
        }

        /// <inheritdoc/>
        public ICondition Condition { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        protected override bool InternalCanExecute(object parameter)
        {
            this.ThrowIfDisposed();
            return base.InternalCanExecute(parameter);
        }

        /// <inheritdoc/>
        protected override void InternalExecute(object parameter)
        {
            this.ThrowIfDisposed();
            base.InternalExecute(parameter);
        }

        /// <summary>
        /// Disposes of a <see cref="ConditionRelayCommand"/>.
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
        /// Throws if the instance has been disposed.
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
