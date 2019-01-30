namespace Gu.Wpf.Reactive.Tests.FakesAndHelpers
{
    using System;
    using System.Reactive.Concurrency;
    using System.Windows;

    public class TestDispatcherScheduler : VirtualTimeSchedulerBase<long, long>
    {
        public override IDisposable ScheduleAbsolute<TState>(TState state, long dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            throw new NotImplementedException();
        }

        protected override long Add(long absolute, long relative)
        {
            throw new NotImplementedException();
        }

        protected override DateTimeOffset ToDateTimeOffset(long absolute)
        {
            throw new NotImplementedException();
        }

        protected override long ToRelative(TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }

        protected override IScheduledItem<long> GetNext()
        {
            _ = Application.Current.Dispatcher.SimulateYield().Wait();
            return null;
        }
    }
}
