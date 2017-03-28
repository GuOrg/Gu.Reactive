namespace Gu.Wpf.Reactive
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Gu.Reactive;

    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/dn630647.aspx
    /// An async command that uses the command parameter
    /// </summary>
    /// <typeparam name="TParameter">The type of the command parameter.</typeparam>
    public class AsyncCommand<TParameter> : ConditionRelayCommand<TParameter>
    {
        private readonly ITaskRunner<TParameter> runner;
        private readonly IDisposable runnerSubscription;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{TParameter}"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        /// <param name="conditions">The conditions for when the command can execute. All must be satisfied.</param>
        public AsyncCommand(Func<TParameter, Task> action, params ICondition[] conditions)
            : this(new TaskRunner<TParameter>(action), conditions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{TParameter}"/> class.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        public AsyncCommand(Func<TParameter, Task> action)
            : this(new TaskRunner<TParameter>(action))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{TParameter}"/> class.
        /// The execution is cancellable.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        /// <param name="conditions">The conditions for when the command can execute. All must be satisfied.</param>
        public AsyncCommand(Func<TParameter, CancellationToken, Task> action, params ICondition[] conditions)
            : this(new TaskRunnerCancelable<TParameter>(action), conditions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand{TParameter}"/> class.
        /// The execution is cancellable.
        /// </summary>
        /// <param name="action">The action to invoke when the command is executed.</param>
        public AsyncCommand(Func<TParameter, CancellationToken, Task> action)
            : this(new TaskRunnerCancelable<TParameter>(action))
        {
        }

        private AsyncCommand(ITaskRunner<TParameter> runner, ICondition[] conditions)
            : this(runner, new AndCondition(runner.CanRunCondition, new AndCondition(conditions, true)))
        {
        }

        private AsyncCommand(ITaskRunner<TParameter> runner)
            : this(runner, runner.CanRunCondition)
        {
        }

        private AsyncCommand(ITaskRunner<TParameter> runner, ICondition condition)
            : base(runner.Run, condition)
        {
            this.runner = runner;
            this.runnerSubscription = runner.ObservePropertyChangedSlim(nameof(runner.TaskCompletion))
                                       .Subscribe(_ => this.OnPropertyChanged(nameof(this.Execution)));

            this.CancelCommand = new ConditionRelayCommand(runner.Cancel, runner.CanCancelCondition);
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
        /// <param name="parameter">The command parameter is passed as argument to the Action invocation.</param>
        protected override async void InternalExecute(TParameter parameter)
        {
            this.IsExecuting = true;
            try
            {
                this.Action(parameter);
                await this.Execution.ObservePropertyChangedSlim(nameof(this.Execution.IsCompleted))
                          .FirstAsync(_ => this.Execution?.IsCompleted == true);
            }
            finally
            {
                this.IsExecuting = false;
            }
        }

        /// <summary>
        /// Disposes of a <see cref="AsyncCommand{T}"/>.
        /// </summary>
        /// <remarks>
        /// Called from Dispose() with disposing=true.
        /// Guidelines:
        /// 1. We may be called more than once: do nothing after the first call.
        /// 2. Avoid throwing exceptions if disposing is false, i.e. if we're being finalized.
        /// </remarks>
        /// <param name="disposing">True if called from Dispose(), false if called from the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.runner.Dispose();
                this.CancelCommand.Dispose();
                this.runnerSubscription?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
