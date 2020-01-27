namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// A command that uses <see cref="CommandManager.RequerySuggested "/> and allows for manually raising CanExecuteChanged.
    /// The command parameter is ignored when using this command.
    /// </summary>
    public class RelayCommand : ManualRelayCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        /// <param name="criteria">The criteria for CanExecute.</param>
        public RelayCommand(Action action, Func<bool> criteria)
            : base(action, criteria)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        public RelayCommand(Action action)
            : this(action, () => true)
        {
        }

        /// <inheritdoc/>
        public override event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                base.CanExecuteChanged += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                base.CanExecuteChanged -= value;
            }
        }
    }
}
