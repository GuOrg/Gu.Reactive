namespace Gu.Wpf.Reactive
{
    using System.Threading.Tasks;

    using Gu.Reactive;

    /// <summary>
    /// Awaits a Task<typeparam name="TResult"/> and makes the result bindable.
    /// http://msdn.microsoft.com/en-us/magazine/dn605875.aspx
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class NotifyTaskCompletion<TResult> : NotifyTaskCompletionBase<Task<TResult>>
    {
        public NotifyTaskCompletion(Task<TResult> task)
            : base(task, NameOf.Property<NotifyTaskCompletion<TResult>, TResult>(x => x.Result))
        {
        }

        public TResult Result
        {
            get
            {
                return (Task.Status == TaskStatus.RanToCompletion)
                    ? Task.Result
                    : default(TResult);
            }
        }
    }

    public sealed class NotifyTaskCompletion : NotifyTaskCompletionBase<Task>
    {
        public NotifyTaskCompletion(Task task)
            : base(task, null)
        {
        }
    }
}

