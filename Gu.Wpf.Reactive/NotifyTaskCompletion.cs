namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    using Gu.Reactive;

    public interface INotifyTaskCompletion : INotifyPropertyChanged 
    {
        object Result { get; }
        Task Task { get; }
        Task Completed { get; }
        TaskStatus Status { get; }
        bool IsCompleted { get; }
        bool IsNotCompleted { get; }
        bool IsSuccessfullyCompleted { get; }
        bool IsCanceled { get; }
        bool IsFaulted { get; }
        AggregateException Exception { get; }
        Exception InnerException { get; }
        string ErrorMessage { get; }
    }

    /// <summary>
    /// Awaits a Task<typeparam name="TResult"/> and makes the result bindable.
    /// http://msdn.microsoft.com/en-us/magazine/dn605875.aspx
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class NotifyTaskCompletion<TResult> : NotifyTaskCompletionBase<Task<TResult>>, INotifyTaskCompletion
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

        object INotifyTaskCompletion.Result
        {
            get
            {
                return Result;
            }
        }

        Task INotifyTaskCompletion.Task
        {
            get
            {
                return Task;
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