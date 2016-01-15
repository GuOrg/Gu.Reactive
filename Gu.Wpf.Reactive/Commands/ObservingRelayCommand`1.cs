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
        private readonly IDisposable _subscription;

        private bool _disposed;

        public ObservingRelayCommand(
            Action<T> action,
            Func<T, bool> criteria,
            params IObservable<object>[] observable)
            : base(action, criteria)
        {
            Ensure.NotNullOrEmpty(observable, nameof(observable));
            _subscription = observable.Merge()
                                      .Subscribe(x => RaiseCanExecuteChanged());
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override bool InternalCanExecute(T parameter)
        {
            VerifyDisposed();
            return base.InternalCanExecute(parameter);
        }

        protected override void InternalExecute(T parameter)
        {
            VerifyDisposed();
            base.InternalExecute(parameter);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _subscription.Dispose();
            }
        }

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}