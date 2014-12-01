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

        protected NotifyTaskCompletionBase(T task, string resultProp) // Not nice at all with string like this.
        {
            this._resultProp = resultProp;
            this.Task = task;
            if (!task.IsCompleted)
            {
                var _ = this.WatchTaskAsync(task);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public T Task { get; private set; }

        public TaskStatus Status { get { return this.Task.Status; } }

        public bool IsCompleted { get { return this.Task.IsCompleted; } }

        public bool IsNotCompleted { get { return !this.Task.IsCompleted; } }

        public bool IsSuccessfullyCompleted
        {
            get
            {
                return this.Task.Status == TaskStatus.RanToCompletion;
            }
        }

        public bool IsCanceled { get { return this.Task.IsCanceled; } }

        public bool IsFaulted { get { return this.Task.IsFaulted; } }

        public AggregateException Exception { get { return this.Task.Exception; } }

        public Exception InnerException
        {
            get
            {
                return (this.Exception == null)
                           ? null
                           : this.Exception.InnerException;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return (this.InnerException == null)
                           ? null
                           : this.InnerException.Message;
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
            var handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, new PropertyChangedEventArgs(NameOf.Property(() => this.Status)));
            handler(this, new PropertyChangedEventArgs(NameOf.Property(() => this.IsCompleted)));
            handler(this, new PropertyChangedEventArgs(NameOf.Property(() => this.IsNotCompleted)));
            if (task.IsCanceled)
            {
                handler(this, new PropertyChangedEventArgs(NameOf.Property(() => this.IsCanceled)));
            }
            else if (task.IsFaulted)
            {
                handler(this, new PropertyChangedEventArgs(NameOf.Property(() => this.IsFaulted)));
                handler(this, new PropertyChangedEventArgs(NameOf.Property(() => this.Exception)));
                handler(this,
                    new PropertyChangedEventArgs(NameOf.Property(() => this.InnerException)));
                handler(this, new PropertyChangedEventArgs(NameOf.Property(() => this.ErrorMessage)));
            }
            else
            {
                handler(this, new PropertyChangedEventArgs(NameOf.Property(() => this.IsSuccessfullyCompleted)));
                if (this._resultProp != null)
                {
                    handler(this, new PropertyChangedEventArgs(this._resultProp));
                }
            }
        }
    }
}