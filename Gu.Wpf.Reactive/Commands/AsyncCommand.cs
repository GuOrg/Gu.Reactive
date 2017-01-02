namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gu.Reactive;

    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// An async command that does not use commandparameter
    /// Returns a Task
    /// </summary>
    public class AsyncCommand : ConditionRelayCommand
    {
        private readonly ITaskRunner runner;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        /// <param name="conditions">The conditions for when the command can execute. All must be satisfied.</param>
        public AsyncCommand(Func<Task> action, params ICondition[] conditions)
            : this(new TaskRunner(action), conditions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        public AsyncCommand(Func<Task> action)
            : this(new TaskRunner(action))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// The execution is cancellable.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        /// <param name="conditions">The conditions for when the command can execute. All must be satisfied.</param>
        public AsyncCommand(Func<CancellationToken, Task> action, params ICondition[] conditions)
            : this(new TaskRunnerCancelable(action), conditions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// The execution is cancellable.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        public AsyncCommand(Func<CancellationToken, Task> action)
            : this(new TaskRunnerCancelable(action))
        {
        }

        private AsyncCommand(ITaskRunner runner, ICondition[] conditions)
            : this(runner, new AndCondition(runner.CanRunCondition, conditions))
        {
        }

        private AsyncCommand(ITaskRunner runner)
            : this(runner, runner.CanRunCondition)
        {
        }

        private AsyncCommand(ITaskRunner runner, ICondition condition)
            : base(runner.Run, condition)
        {
            this.CancelCommand = new ConditionRelayCommand(runner.Cancel, runner.CanCancelCondition);
            this.runner = runner;
            runner.ObservePropertyChangedSlim(nameof(runner.TaskCompletion))
                   .Subscribe(_ => this.OnPropertyChanged(nameof(this.Execution)));
        }

        /// <summary>
        /// A command for cancelling the execution.
        /// This assumes that the command was created with one of the overloads taking a cancellationtoken.
        /// </summary>
        public ConditionRelayCommand CancelCommand { get; }

        /// <summary>
        /// Bindable info about the current execution.
        /// </summary>
        public NotifyTaskCompletion Execution => this.runner.TaskCompletion;

        /// <summary>
        /// Sets IsExecuting to true.
        /// Invokes <see cref="Action"/>
        /// Sets IsExecuting to false.
        /// </summary>
        /// <param name="parameter">The command parameter is ignored by this implementation.</param>
        protected override async void InternalExecute(object parameter)
        {
            this.IsExecuting = true;
            try
            {
                this.Action();
                await this.Execution.ObservePropertyChangedSlim(nameof(this.Execution.IsCompleted))
                               .FirstAsync(_ => this.Execution?.IsCompleted == true);
            }
            finally
            {
                this.IsExecuting = false;
            }
        }
    }
}