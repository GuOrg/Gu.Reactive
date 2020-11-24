namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    using Gu.Reactive;

    /// <summary>
    /// A task runner for non-generic tasks.
    /// </summary>
    public class TaskRunner : TaskRunnerBase, ITaskRunner
    {
        private readonly Func<Task> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskRunner"/> class.
        /// </summary>
        /// <param name="action">The source of tasks to execute.</param>
        public TaskRunner(Func<Task> action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <inheritdoc/>
        public override ICondition CanCancelCondition { get; } = NeverCancelCondition;

        /// <inheritdoc/>
        public void Run()
        {
            this.TaskCompletion = new NotifyTaskCompletion(this.action());
        }
    }
}
