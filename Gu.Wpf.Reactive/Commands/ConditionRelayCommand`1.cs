namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive;

    /// <summary>
    /// A command with CommandParameter of type <typeparam name="T"></typeparam>
    /// Signals CanExecuteChanged when conditions changes
    /// CanExcecute() returns condition.IsSatisfied == true 
    /// </summary>
    public class ConditionRelayCommand<T> : ManualRelayCommand<T>, IConditionRelayCommand
    {
        private readonly IDisposable _subscription;
        private bool _disposed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">SomeMethod</param>
        /// <param name="condition"></param>
        public ConditionRelayCommand(Action<T> action, ICondition condition)
            : base(action, _ => condition.IsSatisfied == true)
        {
            Condition = condition;
            _subscription = Condition.AsObservable()
                                     .Subscribe(_ => RaiseCanExecuteChanged());
        }

        public ICondition Condition { get; }

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
    }
}
