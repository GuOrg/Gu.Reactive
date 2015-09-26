namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive;

    /// <summary>
    /// A command that does not use the CommandParameter
    /// </summary>
    public class ConditionRelayCommand : ManualRelayCommand, IConditionRelayCommand
    {
        private readonly IDisposable _subscription;
        private bool _disposed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">SomeMethod</param>
        /// <param name="condition"></param>
        public ConditionRelayCommand(Action action, ICondition condition)
            : base(action, () => condition.IsSatisfied == true)
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