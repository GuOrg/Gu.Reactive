namespace Gu.Reactive
{
    using System.Reactive.Concurrency;

    public interface ISchedulers
    {
        IScheduler CurrentThread { get; }

        IScheduler Immediate { get; }

        IScheduler NewThread { get; }

        IScheduler TaskPool { get; }

        IScheduler FileSaveScheduler { get; }
    }
}
