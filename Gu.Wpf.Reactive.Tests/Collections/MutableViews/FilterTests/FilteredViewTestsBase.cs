#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews.FilterTests
{
    using System;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;

    public class FilteredViewTestsBase : FilterTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
#pragma warning disable GU0036 // Don't dispose injected.
            this.View?.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
            this.View = new FilteredView<int>(this.Source, x => true, TimeSpan.Zero, this.Scheduler);
        }
    }
}