namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive.Internals;

    /// <summary>
    /// A command with CommandParameter of type <typeparam name="T"></typeparam>
    /// </summary>
    public class ManualRelayCommand<T> : CommandBase<T>
    {
        private static readonly Func<T, bool> AlwaysTrue = _ => true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="criteria"></param>
        public ManualRelayCommand(Action<T> action, Func<T, bool> criteria)
        {
            Ensure.NotNull(action, nameof(action));
            Action = action;
            Criteria = criteria ?? AlwaysTrue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public ManualRelayCommand(Action<T> action)
            : this(action, null)
        {
        }

        protected Action<T> Action { get; }

        protected Func<T, bool> Criteria { get; }

        public bool CanExecute(T parameter)
        {
            return InternalCanExecute(parameter);
        }

        public void Execute(T parameter)
        {
            InternalExecute(parameter);
        }

        protected override bool InternalCanExecute(T parameter)
        {
            return Criteria(parameter);
        }

        protected override void InternalExecute(T parameter)
        {
            IsExecuting = true;
            try
            {
                Action(parameter);
            }
            finally
            {
                IsExecuting = false;
            }
        }
    }
}