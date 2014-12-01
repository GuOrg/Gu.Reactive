namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    /// <summary>
    /// http://msdn.microsoft.com/en-us/magazine/dn605875.aspx
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
    {
        public NotifyTaskCompletion(Task<TResult> task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Task<TResult> Task { get; private set; }

        public TResult Result
        {
            get
            {
                return (Task.Status == TaskStatus.RanToCompletion) ?
                    Task.Result : default(TResult);
            }
        }

        public TaskStatus Status { get { return Task.Status; } }

        public bool IsCompleted { get { return Task.IsCompleted; } }

        public bool IsNotCompleted { get { return !Task.IsCompleted; } }

        public bool IsSuccessfullyCompleted
        {
            get
            {
                return Task.Status ==
                    TaskStatus.RanToCompletion;
            }
        }

        public bool IsCanceled { get { return Task.IsCanceled; } }

        public bool IsFaulted { get { return Task.IsFaulted; } }

        public AggregateException Exception { get { return Task.Exception; } }

        public Exception InnerException
        {
            get
            {
                return (Exception == null) ?
                    null : Exception.InnerException;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return (InnerException == null) ?
                    null : InnerException.Message;
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

            handler(this, new PropertyChangedEventArgs("Status"));
            handler(this, new PropertyChangedEventArgs("IsCompleted"));
            handler(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                handler(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                handler(this, new PropertyChangedEventArgs("IsFaulted"));
                handler(this, new PropertyChangedEventArgs("Exception"));
                handler(this,
                  new PropertyChangedEventArgs("InnerException"));
                handler(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                handler(this,
                  new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
                handler(this, new PropertyChangedEventArgs("Result"));
            }
        }
    }

    public sealed class NotifyTaskCompletion : INotifyPropertyChanged
    {
        public NotifyTaskCompletion(Task task)
        {
            Task = task;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Task Task { get; private set; }

        public TaskStatus Status { get { return Task.Status; } }

        public bool IsCompleted { get { return Task.IsCompleted; } }

        public bool IsNotCompleted { get { return !Task.IsCompleted; } }

        public bool IsSuccessfullyCompleted
        {
            get
            {
                return Task.Status ==
                    TaskStatus.RanToCompletion;
            }
        }

        public bool IsCanceled { get { return Task.IsCanceled; } }

        public bool IsFaulted { get { return Task.IsFaulted; } }

        public AggregateException Exception { get { return Task.Exception; } }

        public Exception InnerException
        {
            get
            {
                return (Exception == null) ?
                    null : Exception.InnerException;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return (InnerException == null) ?
                    null : InnerException.Message;
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

            handler(this, new PropertyChangedEventArgs("Status"));
            handler(this, new PropertyChangedEventArgs("IsCompleted"));
            handler(this, new PropertyChangedEventArgs("IsNotCompleted"));
            if (task.IsCanceled)
            {
                handler(this, new PropertyChangedEventArgs("IsCanceled"));
            }
            else if (task.IsFaulted)
            {
                handler(this, new PropertyChangedEventArgs("IsFaulted"));
                handler(this, new PropertyChangedEventArgs("Exception"));
                handler(this,
                  new PropertyChangedEventArgs("InnerException"));
                handler(this, new PropertyChangedEventArgs("ErrorMessage"));
            }
            else
            {
                handler(this, new PropertyChangedEventArgs("IsSuccessfullyCompleted"));
            }
        }
    }
}

