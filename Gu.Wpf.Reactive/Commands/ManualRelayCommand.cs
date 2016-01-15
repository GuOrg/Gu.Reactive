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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="criteria"></param>
        public ManualRelayCommand(Action action, Func<bool> criteria)
        {
            Ensure.NotNull(action, nameof(action));
            Action = action;
            Criteria = criteria ?? AlwaysTrue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public ManualRelayCommand(Action action)
            : this(action, null)
        {
        }

        protected Action Action { get; }

        protected Func<bool> Criteria { get; }

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
            return Criteria();
        }

        protected override void InternalExecute(object parameter)
        {
            IsExecuting = true;
            try
            {
                Action();
            }
            finally
            {
                IsExecuting = false;
            }
        }
    }
}