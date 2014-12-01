namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive;

    /// <summary>
    /// A command with CommandParameter of type <typeparam name="T"></typeparam>
    /// Signals CanExecuteChanged when conditions changes
    /// CanExcecute() returns condition.IsSatisfied == true 
    /// </summary>
    public class ConditionRelayCommand<T> : ObservingRelayCommand<T>, IConditionRelayCommand
    {
        private readonly ICondition _condition;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">o => CallSomeMethod()</param>
        /// <param name="condition"></param>
        /// <param name="raiseCanExecuteOnDispatcher">default true, use false for tests</param>
        public ConditionRelayCommand(
            Action<T> action,
            ICondition condition,
            bool raiseCanExecuteOnDispatcher = true)
            : base(
                action, 
                _ => condition.IsSatisfied == true, 
                raiseCanExecuteOnDispatcher,
                condition.ToObservable(x => x.IsSatisfied))
        {
            _condition = condition;
        }

        public ICondition Condition
        {
            get
            {
                return _condition;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && Condition != null)
            {
                Condition.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// A command that does not use the CommandParameter
    /// </summary>
    public class ConditionRelayCommand : ObservingRelayCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">o => CallSomeMethod()</param>
        /// <param name="condition"></param>
        /// <param name="raiseCanExecuteOnDispatcher">default true, use false for tests</param>
        public ConditionRelayCommand(Action action, ICondition condition, bool raiseCanExecuteOnDispatcher = true)
            : base(
                action,
                () => condition.IsSatisfied == true,
                raiseCanExecuteOnDispatcher,
                condition.ToObservable(x => x.IsSatisfied))
        {
        }
    }
}
