namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    using Gu.Reactive;

    /// <summary>
    /// A task runner for generic tasks.
    /// </summary>
    /// <typeparam name="TParameter">The type of the command parameter.</typeparam>
    public class TaskRunner<TParameter> : TaskRunnerBase, ITaskRunner<TParameter>
    {
        private readonly Func<TParameter, Task> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskRunner{TParameter}"/> class.
        /// </summary>
        /// <param name="action">The source of tasks to execute.</param>
        public TaskRunner(Func<TParameter, Task> action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <inheritdoc/>
        public override ICondition CanCancelCondition { get; } = NeverCancelCondition;

        /// <inheritdoc/>
        public void Run(TParameter parameter)
        {
            this.TaskCompletion = new NotifyTaskCompletion(this.action(parameter));
        }
    }
}
