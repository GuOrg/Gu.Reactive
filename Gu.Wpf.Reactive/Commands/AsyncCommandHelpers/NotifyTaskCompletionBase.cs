namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// A notifying view of a task execution
    /// </summary>
    /// <typeparam name="T">The type of the task.</typeparam>
    public abstract class NotifyTaskCompletionBase<T> : INotifyPropertyChanged
        where T : Task
    {
        private T completed;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyTaskCompletionBase{T}"/> class.
        /// </summary>
        /// <param name="task">The task to run and notify status for.</param>
        protected NotifyTaskCompletionBase(T task)
        {
            this.Task = task;
            if (task.IsCompleted)
            {
                this.completed = task;
            }
            else
            {
                this.AwaitTask(task);
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The current task.
        /// </summary>
        public T Task { get; }

        /// <summary>
        /// The current status of the <see cref="Task"/>
        /// </summary>
        public TaskStatus Status => this.Task.Status;

        /// <summary>
        /// The current status of the <see cref="Task"/>
        /// </summary>
        public bool IsCompleted => this.Task.IsCompleted;

        /// <summary>
        /// The current status of the <see cref="Task"/>
        /// </summary>
        public bool IsNotCompleted => !this.Task.IsCompleted;

        /// <summary>
        /// The current status of the <see cref="Task"/>
        /// </summary>
        public bool IsSuccessfullyCompleted => this.Task.Status == TaskStatus.RanToCompletion;

        /// <summary>
        /// The current status of the <see cref="Task"/>
        /// </summary>
        public bool IsCanceled => this.Task.IsCanceled;

        /// <summary>
        /// The current status of the <see cref="Task"/>
        /// </summary>
        public bool IsFaulted => this.Task.IsFaulted;

        /// <summary>
        /// The exception produced by the run if any.
        /// </summary>
        public AggregateException Exception => this.Task.Exception;

        /// <summary>
        /// The inner exception produced by the run if any.
        /// </summary>
        public Exception InnerException => this.Exception?.InnerException;

        /// <summary>
        /// The exception message produced by the run if any.
        /// </summary>
        public string ErrorMessage => this.InnerException?.Message;

        /// <summary>
        /// Null if the run is not completed.
        /// </summary>
        public T Completed
        {
            get => this.completed;

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

        /// <summary>
        /// Called after awaiting the task.
        /// </summary>
        protected virtual void OnCompleted()
        {
            this.Completed = this.Task;
        }

        /// <summary>
        /// Notifies that <paramref name="propertyName"/> changed.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
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
                handler(this, new PropertyChangedEventArgs(nameof(this.InnerException)));
                handler(this, new PropertyChangedEventArgs(nameof(this.ErrorMessage)));
            }
        }
    }
}
