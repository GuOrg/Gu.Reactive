namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Linq;

    /// <summary>
    /// A command with CommandParameter of type <typeparam name="T"></typeparam>
    /// Signals CanExecuteChanged when observable signals
    /// </summary>
    public class ObservingRelayCommand<T> : ManualRelayCommand<T>, IDisposable
    {
        private IDisposable _subscription;

        public ObservingRelayCommand(
            Action<T> action,
            Func<T, bool> condition,
            bool raiseCanExecuteOnDispatcher,
            params IObservable<object>[] observable)
            : base(action, condition, raiseCanExecuteOnDispatcher)
        {
            _subscription = observable.Merge()
                                      .Subscribe(x => RaiseCanExecuteChanged());
        }

        public ObservingRelayCommand(
            Action<T> action,
            Func<T, bool> condition,
            params IObservable<object>[] observable)
            : this(action, condition, true, observable)
        {
        }

        public void Dispose()
        {
            Dispose(true);
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

    /// <summary>
    /// A command that does not use the CommandParameter
    /// </summary>
    public class ObservingRelayCommand : ManualRelayCommand, IDisposable
    {
        private IDisposable _subscription;

        public ObservingRelayCommand(
            Action action,
            Func<bool> condition,
            bool raiseCanExecuteOnDispatcher,
            params IObservable<object>[] observable)
            : base(action, condition, raiseCanExecuteOnDispatcher)
        {
            _subscription = observable.Merge()
                                      .Subscribe(x => RaiseCanExecuteChanged());
        }

        public ObservingRelayCommand(
            Action action,
            Func<bool> condition,
            params IObservable<object>[] observable)
            : this(action, condition, true, observable)
        {
        }

        public void Dispose()
        {
            Dispose(true);
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