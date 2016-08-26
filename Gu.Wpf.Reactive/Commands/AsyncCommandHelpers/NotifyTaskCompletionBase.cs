namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public abstract class NotifyTaskCompletionBase<T> : INotifyPropertyChanged
        where T : Task
    {
        private T completed;

        protected NotifyTaskCompletionBase(T task)
        {
            this.Task = task;
            if (task.IsCompleted)
            {
                this.Completed = task;
            }
            else
            {
                this.AwaitTask(task);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public T Task { get; }

        public T Completed
        {
            get { return this.completed; }

            private set
            {
                if (this.completed == value)
                {
                    return;
                }
                this.completed = value;
                this.OnPropertyChanged();
            }
        }

        public TaskStatus Status => this.Task.Status;

        public bool IsCompleted => this.Task.IsCompleted;

        public bool IsNotCompleted => !this.Task.IsCompleted;

        public bool IsSuccessfullyCompleted => this.Task.Status == TaskStatus.RanToCompletion;

        public bool IsCanceled => this.Task.IsCanceled;

        public bool IsFaulted => this.Task.IsFaulted;

        public AggregateException Exception => this.Task.Exception;

        public Exception InnerException => this.Exception?.InnerException;

        public string ErrorMessage => this.InnerException?.Message;

        protected virtual void OnCompleted()
        {
            this.Completed = this.Task;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void AwaitTask(T task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            // ReSharper disable once EmptyGeneralCatchClause We don't want to propagate errors here. Just make them bindable.
            catch
            {
            }

            if (task.Status == TaskStatus.RanToCompletion)
            {
                this.OnCompleted();
            }

            var handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, new PropertyChangedEventArgs(nameof(this.Status)));
            handler(this, new PropertyChangedEventArgs(nameof(this.IsCompleted)));
            handler(this, new PropertyChangedEventArgs(nameof(this.IsNotCompleted)));
            if (task.IsCanceled)
            {
                handler(this, new PropertyChangedEventArgs(nameof(this.IsCanceled)));
            }
            else if (task.IsFaulted)
            {
                handler(this, new PropertyChangedEventArgs(nameof(this.IsFaulted)));
                handler(this, new PropertyChangedEventArgs(nameof(this.Exception)));
                handler(this,
                    new PropertyChangedEventArgs(nameof(this.InnerException)));
                handler(this, new PropertyChangedEventArgs(nameof(this.ErrorMessage)));
            }
        }
    }
}