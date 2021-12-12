namespace Gu.Reactive.Tests.ReadOnlyViews
{
    using System;
    using Microsoft.Reactive.Testing;

    public class ReadOnlyFilteredView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
#pragma warning disable IDISP007 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            this.View = new ReadOnlyFilteredView<int>(this.Source, x => true, TimeSpan.FromMilliseconds(10), this.Scheduler);
            this.Scheduler.Start();
        }
    }
}
