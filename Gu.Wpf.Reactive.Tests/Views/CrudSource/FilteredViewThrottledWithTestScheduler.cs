#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Views.CrudSource
{
    using System;
    using Gu.Reactive.Tests.Collections.ReadOnlyViews;
    using Gu.Reactive.Tests.ReadOnlyViews;
    using Microsoft.Reactive.Testing;

    public class FilteredViewThrottledWithTestScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
#pragma warning disable IDISP007 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            this.View = new FilteredView<int>(this.Source, x => true, TimeSpan.FromMilliseconds(10), this.Scheduler, leaveOpen: false);
            this.Scheduler.Start();
        }
    }
}
