namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    using Gu.Reactive;

    public abstract class NotifyTaskCompletionBase<T> : INotifyPropertyChanged
        where T : Task
    {
        private readonly string _resultProp;
        private readonly TaskCompletionSource<int> _completionSource = new TaskCompletionSource<int>(); 
        protected NotifyTaskCompletionBase(T task, string resultProp) // Not nice at all with string like this.
        {
            Completed = _completionSource.Task;
            _resultProp = resultProp;
            if (!task.IsCompleted)
            {
                var _ = WatchTaskAsync(task);
            }
            else
            {
                _completionSource.SetResult(1); // 1 is not important here
            }
            Task = task;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public T Task { get; }

        public Task Completed { get; }

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

        private async Task WatchTaskAsync(T task)
        {
            try
            {
                await task;
            }
            // ReSharper disable once EmptyGeneralCatchClause. We don't want to propagate errors here. Just make them bindable.
            catch
            {
            }
            _completionSource.SetResult(1); // 1 is not important here
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