namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive.Internals;

    /// <summary>
    /// A command with CommandParameter of type <typeparam name="T"></typeparam>
    /// </summary>
    public class ManualRelayCommand<T> : CommandBase<T>
    {
        private readonly Action<T> _action;
        private readonly Func<T, bool> _condition;

        private static readonly Func<T, bool> AlwaysTrue = _ => true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="condition"></param>
        public ManualRelayCommand(Action<T> action, Func<T, bool> condition)
        {
            Ensure.NotNull(action, nameof(action));
            _action = action;
            _condition = condition ?? AlwaysTrue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public ManualRelayCommand(Action<T> action)
            : this(action, null)
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
}