#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews
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
            this.scheduler = new TestScheduler();
#pragma warning disable GU0036 // Don't dispose injected.
            this.view?.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
            this.view = new FilteredView<int>(this.ints, x => true, TimeSpan.Zero, this.scheduler);
        }
    }
}