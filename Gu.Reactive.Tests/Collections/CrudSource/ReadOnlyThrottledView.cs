namespace Gu.Reactive.Tests.Collections
{
    using System;

    using Microsoft.Reactive.Testing;

    public class ReadOnlyThrottledView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
            (this.View as IDisposable)?.Dispose();
            this.View = new ReadOnlyThrottledView<int>(this.Ints, TimeSpan.FromMilliseconds(10), this.Scheduler);
            this.Scheduler.Start();
        }
    }
}