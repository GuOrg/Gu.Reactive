namespace Gu.Wpf.Reactive
{
    using System.Threading.Tasks;

    /// <summary>
    /// Awaits a <see cref="Task"/> and exposes the result so that it can be bound.
    /// </summary>
    public sealed class NotifyTaskCompletion : NotifyTaskCompletionBase<Task>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyTaskCompletion"/> class.
        /// </summary>
        /// <param name="task">The task to run and notify status for.</param>
        public NotifyTaskCompletion(Task task)
            : base(task)
        {
        }
    }
}
