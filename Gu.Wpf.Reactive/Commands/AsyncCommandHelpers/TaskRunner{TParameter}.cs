namespace Gu.Wpf.Reactive
{
    using System;
    using System.Threading.Tasks;

    using Gu.Reactive.Internals;

    /// <summary>
    /// A taskrunner for generic tasks.
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
            Ensure.NotNull(action, nameof(action));
            this.action = action;
        }

        /// <inheritdoc/>
        public void Run(TParameter parameter)
        {
            this.TaskCompletion = new NotifyTaskCompletion(this.action(parameter));
        }
    }
}
