namespace Gu.Wpf.Reactive
{
    using System.Threading.Tasks;

    public sealed class NotifyTaskCompletion : NotifyTaskCompletionBase<Task>
    {
        public NotifyTaskCompletion(Task task)
            : base(task, null)
        {
        }
    }
}