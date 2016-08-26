namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Linq;

    using Gu.Reactive.Internals;

    /// <summary>
    /// A command with CommandParameter of type <typeparam name="T"></typeparam>
    /// Signals CanExecuteChanged when observable signals
    /// </summary>
    public class ObservingRelayCommand<T> : ManualRelayCommand<T>, IDisposable
    {
        private readonly IDisposable subscription;

        private bool disposed;

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

        protected override bool InternalCanExecute(T parameter)
        {
            this.VerifyDisposed();
            return base.InternalCanExecute(parameter);
        }

        protected override void InternalExecute(T parameter)
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