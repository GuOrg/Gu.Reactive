namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public abstract class NotifyTaskCompletionBase<T> : INotifyPropertyChanged
        where T : Task
    {
        private readonly string _resultProp;
        private readonly TaskCompletionSource<VoidTypeStruct> _completionSource = new TaskCompletionSource<VoidTypeStruct>();

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
                _completionSource.SetResult(new VoidTypeStruct());
            }
            Task = task;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public T Task { get; }

        public Task Completed { get; }

        public TaskStatus Status => Task.Status;

        public bool IsCompleted => Task.IsCompleted;

        public bool IsNotCompleted => !Task.IsCompleted;

        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

        public bool IsCanceled => Task.IsCanceled;

        public bool IsFaulted => Task.IsFaulted;

        public AggregateException Exception => Task.Exception;

        public Exception InnerException => Exception?.InnerException;

        public string ErrorMessage => InnerException?.Message;

        private async Task WatchTaskAsync(T task)
        {
            try
            {
                await task;
            }
            // ReSharper disable once EmptyGeneralCatchClause We don't want to propagate errors here. Just make them bindable.
            catch
            {
            }
            _completionSource.SetResult(new VoidTypeStruct());
            var handler = PropertyChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, new PropertyChangedEventArgs(nameof(Status)));
            handler(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
            handler(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));
            if (task.IsCanceled)
            {
                handler(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
            }
            else if (task.IsFaulted)
            {
                handler(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
                handler(this, new PropertyChangedEventArgs(nameof(Exception)));
                handler(this,
                    new PropertyChangedEventArgs(nameof(InnerException)));
                handler(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
            }
            else
            {
                handler(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
                if (_resultProp != null)
                {
                    handler(this, new PropertyChangedEventArgs(_resultProp));
                }
            }
        }
    }
}