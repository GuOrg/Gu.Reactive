namespace Gu.Reactive
{
    using System.Reactive.Concurrency;

    /// <summary>
    /// <see cref="Gu.Reactive.ISchedulers"/> with <see cref="Dispatcher"/>.
    /// </summary>
    public interface IWpfSchedulers : ISchedulers
    {
        /// <summary>
        /// The dispatcher scheduler for the current application.
        /// </summary>
        IScheduler Dispatcher { get; }
    }
}