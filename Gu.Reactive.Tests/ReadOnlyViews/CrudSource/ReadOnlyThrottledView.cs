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
#pragma warning disable IDISP007 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            this.View = new ReadOnlyThrottledView<int>(this.Source, TimeSpan.FromMilliseconds(10), this.Scheduler, leaveOpen: false);
            this.Scheduler.Start();
        }
    }
}