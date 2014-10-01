namespace Gu.Wpf.Reactive
{
    using System;

    public class ObservingRelayCommand : ManualRelayCommand, IDisposable
    {
        private IDisposable _subscription;

        public ObservingRelayCommand(
            Action<object> action,
            Predicate<object> condition,
            IObservable<object> observable,
            bool raiseCanExecuteOnDispatcher = true)
            : base(action, condition, raiseCanExecuteOnDispatcher)
        {
            _subscription = observable.Subscribe(x => this.RaiseCanExecuteChanged());
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _subscription != null)
            {
                _subscription.Dispose();
                _subscription = null;
            }
        }
    }
}