namespace Gu.Reactive
{
    using System.Reactive.Concurrency;

    /// <summary>
    /// For replacing with TestScheduler in unit tests.
    /// </summary>
    public interface ISchedulers
    {
        /// <summary>
        /// Gets a scheduler that schedules work as soon as possible on the current thread.
        /// <see cref="Schedulers.CurrentThread"/>
        /// </summary>
        IScheduler CurrentThread { get; }

        /// <summary>
        /// Gets a scheduler that schedules work immediately on the current thread.
        /// <see cref="Schedulers.Immediate"/>
        /// </summary>
        IScheduler Immediate { get; }

        /// <summary>
        /// Gets a scheduler that schedules work on a new thread using default thread creation options.
        /// <see cref="Schedulers.NewThread"/>
        /// </summary>
        IScheduler NewThread { get; }

        /// <summary>
        /// Gets a scheduler that schedules work on Task Parallel Library (TPL) task pool using the default TaskScheduler.
        /// <see cref="Schedulers.TaskPool"/>
        /// </summary>
        IScheduler TaskPool { get; }

        /// <summary>
        /// Gets a scheduler that runs on a dedicated thread and with IsBackground = false while there is work left.
        /// This can be useful for operations that needs to run to completion before application exits.
        /// <see cref="Schedulers.TaskPool"/>
        /// </summary>
        IScheduler FileSaveScheduler { get; }
    }
}
