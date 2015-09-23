namespace Gu.Reactive
{
    using System.Reactive.Concurrency;

    public interface IWpfSchedulers : ISchedulers
    {
        IScheduler Dispatcher { get; }
    }
}