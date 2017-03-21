namespace Gu.Reactive.Tests.Collections.ReadOnlyViews
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
            this.View = new ReadOnlyThrottledView<int>(this.Source, TimeSpan.FromMilliseconds(10), this.Scheduler, false);
            this.Scheduler.Start();
        }
    }
}