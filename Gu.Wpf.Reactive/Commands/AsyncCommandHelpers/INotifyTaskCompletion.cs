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
        /// The current status of the <see cref="Task"/>.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// The current status of the <see cref="Task"/>.
        /// </summary>
        bool IsNotCompleted { get; }

        /// <summary>
        /// The current status of the <see cref="Task"/>.
        /// </summary>
        bool IsSuccessfullyCompleted { get; }

        /// <summary>
        /// The current status of the <see cref="Task"/>.
        /// </summary>
        bool IsCanceled { get; }

        /// <summary>
        /// The current status of the <see cref="Task"/>.
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
