namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive;

    /// <summary>
    /// A command that does not use the CommandParameter
    /// </summary>
    public class ConditionRelayCommand : ManualRelayCommand, IConditionRelayCommand
    {
        private readonly IDisposable subscription;
        private bool disposed;

        /// <summary>
        ///
        /// </summary>
        /// <param name="action">SomeMethod</param>
        /// <param name="condition"></param>
        public ConditionRelayCommand(Action action, ICondition condition)
            : base(action, () => condition.IsSatisfied == true)
        {
            this.Condition = condition;
            this.subscription = this.Condition.ObserveIsSatisfiedChanged()
                                     .Subscribe(_ => this.RaiseCanExecuteChanged());
        }

        public ICondition Condition { get; }

        public void Dispose()
        {
            if (this.disposed)
            {
                return;
            }
            this.disposed = true;
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override bool InternalCanExecute(object parameter)
        {
            this.VerifyDisposed();
            return base.InternalCanExecute(parameter);
        }

        protected override void InternalExecute(object parameter)
        {
            this.VerifyDisposed();
            base.InternalExecute(parameter);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.subscription.Dispose();
            }
        }

        protected void VerifyDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}