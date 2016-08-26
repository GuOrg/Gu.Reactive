namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Linq;

    using Gu.Reactive.Internals;

    /// <summary>
    /// A command that does not use the CommandParameter
    /// </summary>
    public class ObservingRelayCommand : ManualRelayCommand, IDisposable
    {
        private readonly IDisposable subscription;
        private bool disposed;

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