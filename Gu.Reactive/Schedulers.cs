namespace Gu.Reactive
{
    using System.Reactive.Concurrency;

    /// <inheritdoc/>
    public class Schedulers : ISchedulers
    {
        /// <inheritdoc/>
        public IScheduler CurrentThread => Scheduler.CurrentThread;

        /// <inheritdoc/>
        public IScheduler Immediate => Scheduler.Immediate;

        /// <inheritdoc/>
        public IScheduler NewThread => NewThreadScheduler.Default;

        /// <inheritdoc/>
        public IScheduler TaskPool => TaskPoolScheduler.Default;

        /// <inheritdoc/>
        public IScheduler FileSaveScheduler => ForegroundScheduler.Default;
    }
}
