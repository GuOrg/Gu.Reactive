namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows.Input;

    /// <inheritdoc/>
    public class RelayCommand<T> : ManualRelayCommand<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        /// <param name="criteria">The criteria for CanExecute.</param>
        public RelayCommand(Action<T?> action, Func<T?, bool> criteria)
            : base(action, criteria)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand{T}"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        public RelayCommand(Action<T?> action)
            : this(action, o => true)
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
