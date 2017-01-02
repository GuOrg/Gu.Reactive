namespace Gu.Wpf.Reactive
{
    using System;

    using Gu.Reactive.Internals;

    /// <summary>
    /// A command with CommandParameter of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    public class ManualRelayCommand<T> : CommandBase<T>
    {
        private static readonly Func<T, bool> AlwaysTrue = _ => true;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualRelayCommand{T}"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        /// <param name="criteria">The criteria for <see cref="CanExecute"/></param>
        public ManualRelayCommand(Action<T> action, Func<T, bool> criteria)
        {
            Ensure.NotNull(action, nameof(action));
            this.Action = action;
            this.Criteria = criteria ?? AlwaysTrue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualRelayCommand{T}"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        public ManualRelayCommand(Action<T> action)
            : this(action, null)
        {
        }

        protected Action<T> Action { get; }

        protected Func<T, bool> Criteria { get; }

        public bool CanExecute(T parameter)
        {
            return this.InternalCanExecute(parameter);
        }

        public void Execute(T parameter)
        {
            this.InternalExecute(parameter);
        }

        protected override bool InternalCanExecute(T parameter)
        {
            return this.Criteria(parameter);
        }

        protected override void InternalExecute(T parameter)
        {
            this.IsExecuting = true;
            try
            {
                this.Action(parameter);
            }
            finally
            {
                this.IsExecuting = false;
            }
        }
    }
}