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
        private readonly IDisposable _subscription;

        private bool _disposed;

        public ObservingRelayCommand(
            Action action,
            Func<bool> condition,
            params IObservable<object>[] observable)
            : base(action, condition)
        {
            Ensure.NotNullOrEmpty(observable, nameof(observable));
            _subscription = observable.Merge()
                                      .Subscribe(_ => RaiseCanExecuteChanged());
        }

        protected override bool InternalCanExecute(object parameter)
        {
            VerifyDisposed();
            return base.InternalCanExecute(parameter);
        }

        protected override void InternalExecute(object parameter)
        {
            VerifyDisposed();
            base.InternalExecute(parameter);
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