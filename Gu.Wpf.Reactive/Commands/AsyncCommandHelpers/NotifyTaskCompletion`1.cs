namespace Gu.Wpf.Reactive
{
    using System.Threading.Tasks;

    /// <summary>
    /// Awaits a Task<typeparam name="TResult"/> and makes the result bindable.
    /// http://msdn.microsoft.com/en-us/magazine/dn605875.aspx
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class NotifyTaskCompletion<TResult> : NotifyTaskCompletionBase<Task<TResult>>, INotifyTaskCompletion
    {
        public NotifyTaskCompletion(Task<TResult> task)
            : base(task)
        {
        }

        Task INotifyTaskCompletion.Task => Task;

        Task INotifyTaskCompletion.Completed => Completed;
    }
}