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
        /// <summary>
        /// A condition that always returns false.
        /// </summary>
        protected static readonly Condition NeverCancelCondition = new Condition(Observable.Empty<object>(), () => false) { Name = "CanCancel" };
        private NotifyTaskCompletion? taskCompletion;

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskRunnerBase"/> class.
        /// </summary>
        protected TaskRunnerBase()
        {
            this.CanRunCondition = new Condition(
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                this.ObservePropertyChanged(x => x.TaskCompletion.Status),
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                this.CanRun)
            {
                Name = "CanRun",
            };
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the condition for if the current run can be canceled.
        /// </summary>
        public abstract ICondition CanCancelCondition { get; }

        /// <summary>
        /// Gets the condition for if the task can be executed.
        /// </summary>
        public ICondition CanRunCondition { get; }

        /// <summary>
        /// Gets or sets the status of the current task.
        /// </summary>
        public NotifyTaskCompletion? TaskCompletion
        {
            get => this.taskCompletion;

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
        /// Check if the task is running.
        /// </summary>
        /// <returns>True if execution can be started.</returns>
        public bool? CanRun()
        {
            this.ThrowIfDisposed();
            var completion = this.TaskCompletion;
            if (completion is null)
            {
                return true;
            }

#pragma warning disable IDE0066 // Convert switch statement to expression
            switch (completion.Task.Status)
#pragma warning restore IDE0066 // Convert switch statement to expression
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
                    throw new InvalidOperationException($"Should never get here. Unhandled status: {completion.Task.Status}");
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
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if (disposing)
            {
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
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
