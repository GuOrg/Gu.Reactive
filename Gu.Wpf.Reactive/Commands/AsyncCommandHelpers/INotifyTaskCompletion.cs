namespace Gu.Wpf.Reactive
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    /// <summary>
    /// A notifying view of a task execution.
    /// </summary>
    public interface INotifyTaskCompletion : INotifyPropertyChanged
    {
        /// <summary>
        /// The task.
        /// </summary>
        Task Task { get; }

        /// <summary>
        /// Null if the run is not completed.
        /// </summary>
        Task? Completed { get; }

        /// <summary>
        /// The current status of the <see cref="Task"/>.
        /// </summary>
        TaskStatus Status { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Task"/> is completed.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Task"/> is not completed.
        /// </summary>
        bool IsNotCompleted { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Task"/> is successfully completed.
        /// </summary>
        bool IsSuccessfullyCompleted { get; }

        /// <summary>
        /// The current status of the <see cref="Task"/>.
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Task"/> is faulted.
        /// </summary>
        bool IsFaulted { get; }

        /// <summary>
        /// The exception produced by the run if any.
        /// </summary>
        AggregateException? Exception { get; }

        /// <summary>
        /// The inner exception produced by the run if any.
        /// </summary>
        Exception? InnerException { get; }

        /// <summary>
        /// The exception message produced by the run if any.
        /// </summary>
        string? ErrorMessage { get; }
    }
}
