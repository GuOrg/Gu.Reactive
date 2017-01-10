namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Gu.Reactive;

    /// <summary>
    /// A base class for running tasks and notifying about the results.
    /// </summary>
    public abstract class TaskRunnerBase : INotifyPropertyChanged, IDisposable
    {
        private NotifyTaskCompletion taskCompletion;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskRunnerBase"/> class.
        /// </summary>
        protected TaskRunnerBase()
        {
            var observable = this.ObservePropertyChanged(x => x.TaskCompletion.Status);
            this.CanRunCondition = new Condition(observable, this.CanRun) { Name = "CanRun" };
            this.CanCancelCondition = new Condition(Observable.Empty<object>(), () => false) { Name = "CanCancel" };
        }

        /// <inheritdoc/>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The status of the current task.
        /// </summary>
        public NotifyTaskCompletion TaskCompletion
        {
            get
            {
                return this.taskCompletion;
            }

            protected set
            {
                if (ReferenceEquals(value, this.taskCompletion))
                {
                    return;
                }

                this.taskCompletion = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Condition for if the current run can be canceled.
        /// </summary>
        public virtual ICondition CanCancelCondition { get; }

        /// <summary>
        /// Condition for if the task can be executed.
        /// </summary>
        public ICondition CanRunCondition { get; }

        /// <summary>
        /// Check if the task is running.
        /// </summary>
        /// <returns>True if execution can be started.</returns>
        public bool? CanRun()
        {
            this.ThrowIfDisposed();
            var completion = this.TaskCompletion;
            if (completion == null)
            {
                return true;
            }

            switch (completion.Task.Status)
            {
                case TaskStatus.Created:
                case TaskStatus.WaitingForActivation:
                case TaskStatus.WaitingToRun:
                case TaskStatus.Running:
                case TaskStatus.WaitingForChildrenToComplete:
                    return false;
                case TaskStatus.RanToCompletion:
                case TaskStatus.Canceled:
                case TaskStatus.Faulted:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Cancel the execution.
        /// </summary>
        public virtual void Cancel()
        {
            // intentional NOP
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
                this.TaskCompletion?.Task.Dispose();
                this.CanCancelCondition?.Dispose();
                this.CanRunCondition.Dispose();
            }
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the instance is disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        /// Notifies that <paramref name="propertyName"/> changed.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}