namespace Gu.Reactive.Tests.ReadOnlyViews
{
    using System;
    using Microsoft.Reactive.Testing;

    public class ReadOnlyFilteredViewMappedView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
#pragma warning disable IDISP007 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            this.View = this.Source.AsReadOnlyFilteredView(x => true, TimeSpan.FromMilliseconds(10), this.Scheduler)
                            .AsMappingView(x => x);
            this.Scheduler.Start();
        }
    }
}
