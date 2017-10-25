#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews.CrudView
{
    using System;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class FilteredViewThrottledWithTestScheduler : CrudViewTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
#pragma warning disable IDISP007 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            this.View = new FilteredView<int>(this.Ints, x => true, TimeSpan.FromMilliseconds(10), this.Scheduler, leaveOpen: false);
        }
    }
}