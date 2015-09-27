namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive.Internals;

    /// <summary>
    /// A command that does not use the CommandParameter
    /// </summary>
    public class ManualRelayCommand : CommandBase<object>
    {
        private static readonly Func<bool> AlwaysTrue = () => true;
        private readonly Action _action;
        private readonly Func<bool> _condition;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        public ManualRelayCommand(Action action, Func<bool> condition)
        {
            Ensure.NotNull(action, nameof(action));
            _action = action;
            _condition = condition ?? AlwaysTrue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public ManualRelayCommand(Action action)
            : this(action, null)
        {
        }

        public bool CanExecute()
        {
            // Override InternalCanExecute
            return InternalCanExecute(null);
        }

        public void Execute()
        {
            // Override InternalExecute
            InternalExecute(null);
        }

        protected override bool InternalCanExecute(object parameter)
        {
            return _condition();
        }

        protected override void InternalExecute(object parameter)
        {
            _action();
        }
    }
}