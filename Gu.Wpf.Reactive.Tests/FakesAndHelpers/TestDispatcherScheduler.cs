namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System;
    using System.Reactive.Concurrency;
    using System.Windows;

    public class TestDispatcherScheduler : VirtualTimeSchedulerBase<long, long>
    {
        public override IDisposable ScheduleAbsolute<TState>(TState state, long dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            throw new InvalidOperationException("Not meant to be used.");
        }

        protected override long Add(long absolute, long relative)
        {
            throw new InvalidOperationException("Not meant to be used.");
        }

        protected override DateTimeOffset ToDateTimeOffset(long absolute)
        {
            throw new InvalidOperationException("Not meant to be used.");
        }

        protected override long ToRelative(TimeSpan timeSpan)
        {
            throw new InvalidOperationException("Not meant to be used.");
        }

        protected override IScheduledItem<long>? GetNext()
        {
            _ = Application.Current.Dispatcher.SimulateYield().Wait();
            return null;
        }
    }
}
