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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">o => CallSomeMethod()</param>
        /// <param name="condition"></param>
        public ConditionRelayCommand(
            Action<T> action,
            ICondition condition)
            : base(
                action, 
                _ => condition.IsSatisfied == true, 
                condition.ObservePropertyChanged(x => x.IsSatisfied))
        {
            Condition = condition;
        }

        public ICondition Condition { get; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Condition?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// A command that does not use the CommandParameter
    /// </summary>
    public class ConditionRelayCommand : ObservingRelayCommand, IConditionRelayCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">o => CallSomeMethod()</param>
        /// <param name="condition"></param>
        public ConditionRelayCommand(Action action, ICondition condition)
            : base(
                action,
                () => condition.IsSatisfied == true,
                condition.ObservePropertyChanged(x => x.IsSatisfied))
        {
            Condition = condition;
        }

        public ICondition Condition { get; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Condition?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
