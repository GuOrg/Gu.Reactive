namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
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

    public abstract class NotifyTaskCompletionBase<T> : INotifyPropertyChanged
        where T : Task
    {
        private readonly string _resultProp;

        protected NotifyTaskCompletionBase(T task, string resultProp) // Not nice at all with string like this.
        {
            _resultProp = resultProp;
            Task = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public T Task { get; private set; }

        public TaskStatus Status { get { return Task.Status; } }

        public bool IsCompleted { get { return Task.IsCompleted; } }

        public bool IsNotCompleted { get { return !Task.IsCompleted; } }

        public bool IsSuccessfullyCompleted
        {
            get
            {
                return Task.Status == TaskStatus.RanToCompletion;
            }
        }

        public bool IsCanceled { get { return Task.IsCanceled; } }

        public bool IsFaulted { get { return Task.IsFaulted; } }

        public AggregateException Exception { get { return Task.Exception; } }

        public Exception InnerException
        {
            get
            {
                return (Exception == null)
                    ? null
                    : Exception.InnerException;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return (InnerException == null)
                    ? null
                    : InnerException.Message;
            }
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            // ReSharper disable once EmptyGeneralCatchClause. We don't want to propagate errors here. Just make them bindable.
            catch
            {
            }
            var handler = PropertyChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, new PropertyChangedEventArgs(NameOf.Property(() => Status)));
            handler(this, new PropertyChangedEventArgs(NameOf.Property(() => IsCompleted)));
            handler(this, new PropertyChangedEventArgs(NameOf.Property(() => IsNotCompleted)));
            if (task.IsCanceled)
            {
                handler(this, new PropertyChangedEventArgs(NameOf.Property(() => IsCanceled)));
            }
            else if (task.IsFaulted)
            {
                handler(this, new PropertyChangedEventArgs(NameOf.Property(() => IsFaulted)));
                handler(this, new PropertyChangedEventArgs(NameOf.Property(() => Exception)));
                handler(this,
                  new PropertyChangedEventArgs(NameOf.Property(() => InnerException)));
                handler(this, new PropertyChangedEventArgs(NameOf.Property(() => ErrorMessage)));
            }
            else
            {
                handler(this, new PropertyChangedEventArgs(NameOf.Property(() => IsSuccessfullyCompleted)));
                if (_resultProp != null)
                {
                    handler(this, new PropertyChangedEventArgs(_resultProp));
                }
            }
        }
    }
}

