namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    using Gu.Reactive.Internals;

    /// <summary>
    /// A taskrunner for nongeneric tasks.
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
            Ensure.NotNull(action, nameof(action));
            this.action = action;
        }

        /// <inheritdoc/>
        public void Run()
        {
            this.TaskCompletion = new NotifyTaskCompletion(this.action());
        }
    }
}