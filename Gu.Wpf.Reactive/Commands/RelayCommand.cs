namespace Gu.Wpf.Reactive
{
    using System;
    using System.Windows.Input;

    public class RelayCommand : ManualRelayCommand
    {
        public RelayCommand(Action action, Func<bool> criteria)
            : base(action, criteria)
        {
        }

        public RelayCommand(Action action)
            : this(action, () => true)
        {
        }

        /// <summary>
        /// http://stackoverflow.com/a/2588145/1069200
        /// </summary>
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