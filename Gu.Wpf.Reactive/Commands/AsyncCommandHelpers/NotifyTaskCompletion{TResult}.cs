namespace Gu.Wpf.Reactive
{
    using System.Threading.Tasks;

    /// <summary>
    /// Awaits a <see cref="Task{TResult}"/> and makes the result bindable.
    /// http://msdn.microsoft.com/en-us/magazine/dn605875.aspx
    /// </summary>
    /// <typeparam name="TResult">The type of the task.</typeparam>
    public sealed class NotifyTaskCompletion<TResult> : NotifyTaskCompletionBase<Task<TResult>>, INotifyTaskCompletion
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyTaskCompletion{TResult}"/> class.
        /// </summary>
        /// <param name="task">The task to run and notify status for.</param>
        public NotifyTaskCompletion(Task<TResult> task)
            : base(task)
        {
        }

        /// <inheritdoc/>
        Task INotifyTaskCompletion.Task => this.Task;

        /// <inheritdoc/>
        Task INotifyTaskCompletion.Completed => this.Completed;
    }
}