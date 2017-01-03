namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive;

    /// <summary>
    /// A command with CommandParameter of type <typeparamref name="T"/>.
    /// Signals CanExecuteChanged when conditions changes
    /// CanExecute() returns condition.IsSatisfied == true
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    public class ConditionRelayCommand<T> : ManualRelayCommand<T>, IConditionRelayCommand
    {
        private readonly IDisposable subscription;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionRelayCommand{T}"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        /// <param name="condition">The criteria by CanExecute</param>
        public ConditionRelayCommand(Action<T> action, ICondition condition)
            : base(action, _ => condition.IsSatisfied == true)
        {
            this.Condition = condition;
            this.subscription = this.Condition.ObserveIsSatisfiedChanged()
                                     .Subscribe(_ => this.RaiseCanExecuteChanged());
        }

        /// <inheritdoc/>
        public ICondition Condition { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <inheritdoc/>
        protected override bool InternalCanExecute(T parameter)
        {
            this.VerifyDisposed();
            return base.InternalCanExecute(parameter);
        }

        /// <inheritdoc/>
        protected override void InternalExecute(T parameter)
        {
            this.VerifyDisposed();
            base.InternalExecute(parameter);
        }

        /// <summary>
        /// Disposes of a <see cref="ConditionRelayCommand{T}"/>.
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
