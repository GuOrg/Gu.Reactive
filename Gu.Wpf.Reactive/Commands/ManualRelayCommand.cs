namespace Gu.Wpf.Reactive
{
    using System;

    /// <summary>
    /// A command with CommandParameter of type <typeparam name="T"></typeparam>
    /// </summary>
    public class ManualRelayCommand<T> : ManualCommandBase<T>
    {
        private readonly Action<T> _action;
        private readonly Func<T, bool> _condition;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        public ManualRelayCommand(Action<T> action, Func<T, bool> condition)
        {
            _action = action;
            _condition = condition ?? (_ => true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public ManualRelayCommand(Action<T> action)
            : this(action, _ => true)
        {
        }

        public bool CanExecute(T parameter)
        {
            return InternalCanExecute(parameter);
        }

        public void Execute(T parameter)
        {
            InternalExecute(parameter);
        }

        protected override void InternalExecute(T parameter)
        {
            _action(parameter);
            RaiseCanExecuteChanged();
        }

        protected override bool InternalCanExecute(T parameter)
        {
            return _condition(parameter);
        }
    }

    /// <summary>
    /// A command that does not use the CommandParameter
    /// </summary>
    public class ManualRelayCommand : ManualCommandBase<object>
    {
        private readonly Action _action;
        private readonly Func<bool> _condition;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        /// <param name="raiseCanExecuteOnDispatcher">Default true, use false in tests</param>
        public ManualRelayCommand(Action action, Func<bool> condition)
        {
            _action = action;
            _condition = condition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="raiseCanExecuteOnDispatcher">Default true, use false in tests</param>
        public ManualRelayCommand(Action action)
            : this(action, () => true)
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