namespace Gu.Reactive
{
    using System.Reactive.Concurrency;

    public class Schedulers : ISchedulers
    {
        public IScheduler CurrentThread => Scheduler.CurrentThread;

        public IScheduler Immediate => Scheduler.Immediate;

        public IScheduler NewThread => NewThreadScheduler.Default;

        public IScheduler TaskPool => TaskPoolScheduler.Default;

        public IScheduler FileSaveScheduler => ForegroundScheduler.Default;
    }
}