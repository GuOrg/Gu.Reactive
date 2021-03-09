#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.Views.CrudSource
{
    using System;
    using Gu.Reactive.Tests.Collections.ReadOnlyViews;
    using Microsoft.Reactive.Testing;

    public class ThrottledViewWithScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
#pragma warning disable IDISP007 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            this.View = new ThrottledView<int>(this.Source, TimeSpan.FromMilliseconds(10), this.Scheduler, leaveOpen: false);
            this.Scheduler.Start();
        }
    }
}