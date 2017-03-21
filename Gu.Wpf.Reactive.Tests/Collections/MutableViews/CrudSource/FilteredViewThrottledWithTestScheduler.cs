#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews.CrudSource
{
    using System;

    using Gu.Reactive.Tests.Collections.ReadOnlyViews;

    using Microsoft.Reactive.Testing;

    public class FilteredViewThrottledWithTestScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
            (this.View as IDisposable)?.Dispose();
            this.View = new FilteredView<int>(this.Source, x => true, TimeSpan.FromMilliseconds(10), this.Scheduler, false);
            this.Scheduler.Start();
        }
    }
}