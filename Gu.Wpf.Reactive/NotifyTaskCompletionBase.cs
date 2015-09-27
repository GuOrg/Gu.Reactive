namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public abstract class NotifyTaskCompletionBase<T> : INotifyPropertyChanged
        where T : Task
    {
        private T _completed;

        protected NotifyTaskCompletionBase(T task)
        {
            Task = task;
            if (task.IsCompleted)
            {
                Completed = task;
            }
            else
            {
                AwaitTask(task);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public T Task { get; }

        public T Completed
        {
            get { return _completed; }
            private set
            {
                if (_completed == value)
                {
                    return;
                }
                _completed = value;
                OnPropertyChanged();
            }
        }

        public TaskStatus Status => Task.Status;

        public bool IsCompleted => Task.IsCompleted;

        public bool IsNotCompleted => !Task.IsCompleted;

        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

        public bool IsCanceled => Task.IsCanceled;

        public bool IsFaulted => Task.IsFaulted;

        public AggregateException Exception => Task.Exception;

        public Exception InnerException => Exception?.InnerException;

        public string ErrorMessage => InnerException?.Message;

        protected virtual void OnCompleted()
        {
            Completed = Task;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void AwaitTask(T task)
        {
            try
            {
                await task;
            }
            // ReSharper disable once EmptyGeneralCatchClause We don't want to propagate errors here. Just make them bindable.
            catch
            {
            }
            if (task.Status == TaskStatus.RanToCompletion)
            {
                OnCompleted();
            }
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
        }
    }
}