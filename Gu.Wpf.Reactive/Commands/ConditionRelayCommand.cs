namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive;

    /// <summary>
    /// A command that does not use the CommandParameter
    /// </summary>
    public class ConditionRelayCommand : ObservingRelayCommand, IConditionRelayCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">SomeMethod</param>
        /// <param name="condition"></param>
        public ConditionRelayCommand(Action action, ICondition condition)
            : base(action, () => condition.IsSatisfied == true, condition.AsObservable())
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