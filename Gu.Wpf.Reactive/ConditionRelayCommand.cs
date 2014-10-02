namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive;

    /// <summary>
    /// Signals CanExecuteChanged when conditions changes
    /// CanExcecute() returnerar condition.IsSatisfied == true 
    /// </summary>
    public class ConditionRelayCommand : ManualRelayCommand, IDisposable
    {
        private IDisposable _subscription;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">o => CallSomeMethod()</param>
        /// <param name="condition"></param>
        /// <param name="raiseCanExecuteOnDispatcher">default true, use false for tests</param>
        public ConditionRelayCommand(
            Action<object> action,
            ICondition condition,
            bool raiseCanExecuteOnDispatcher = true)
            : base(action, o => condition.IsSatisfied == true, raiseCanExecuteOnDispatcher)
        {
            Condition = condition;
            _subscription = condition.ToObservable(x => x.IsSatisfied)
                                     .Subscribe(x => this.RaiseCanExecuteChanged());
        }
        public ICondition Condition { get; private set; }

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
