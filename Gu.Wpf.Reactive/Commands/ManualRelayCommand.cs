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
            this.Action = action;
            this.Criteria = criteria ?? AlwaysTrue;
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
            return this.InternalCanExecute(null);
        }

        public void Execute()
        {
            // Override InternalExecute
            this.InternalExecute(null);
        }

        protected override bool InternalCanExecute(object parameter)
        {
            return this.Criteria();
        }

        protected override void InternalExecute(object parameter)
        {
            this.IsExecuting = true;
            try
            {
                this.Action();
            }
            finally
            {
                this.IsExecuting = false;
            }
        }
    }
}