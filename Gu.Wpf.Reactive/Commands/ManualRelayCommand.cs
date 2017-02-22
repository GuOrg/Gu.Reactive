namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive.Internals;
    using Gu.Reactive.Internals.Ensure;

    /// <summary>
    /// A command that does not use the CommandParameter
    /// The command parameter is ignored when using this command.
    /// </summary>
    public class ManualRelayCommand : CommandBase<object>
    {
        private static readonly Func<bool> AlwaysTrue = () => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualRelayCommand"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        /// <param name="criteria">The criteria for <see cref="CanExecute"/></param>
        public ManualRelayCommand(Action action, Func<bool> criteria)
        {
            Ensure.NotNull(action, nameof(action));
            this.Action = action;
            this.Criteria = criteria ?? AlwaysTrue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualRelayCommand"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        public ManualRelayCommand(Action action)
            : this(action, null)
        {
        }

        /// <summary>
        /// The action to invoke when the command is executed.
        /// </summary>
        protected Action Action { get; }

        /// <summary>
        /// The criteria for <see cref="CanExecute"/>
        /// </summary>
        protected Func<bool> Criteria { get; }

        /// <summary>
        /// Calls <see cref="InternalCanExecute"/> to see if the command can execute.
        /// </summary>
        /// <returns>True if the command can execute.</returns>
        public bool CanExecute()
        {
            // Override InternalCanExecute
            return this.InternalCanExecute(null);
        }

        /// <summary>
        /// Calls <see cref="InternalExecute"/>.
        /// </summary>
        public void Execute()
        {
            // Override InternalExecute
            this.InternalExecute(null);
        }

        /// <summary>
        /// Evaluates <see cref="Criteria"/> to see if the command can execute.
        /// </summary>
        /// <param name="parameter">The command parameter is ignored by this implementation.</param>
        /// <returns>A value indicating if the command can execute.</returns>
        protected override bool InternalCanExecute(object parameter)
        {
            return this.Criteria();
        }

        /// <summary>
        /// Sets IsExecuting to true.
        /// Invokes <see cref="Action"/>
        /// Sets IsExecuting to false.
        /// </summary>
        /// <param name="parameter">The command parameter is ignored by this implementation.</param>
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