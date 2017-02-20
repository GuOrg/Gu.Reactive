namespace Gu.Reactive.Tests.Collections.Filter
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class FilteredViewNoScheduler : FilterTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
#pragma warning disable GU0036 // Don't dispose injected.
            this.view?.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
            this.view = this.ints.AsFilteredView(x => true, TimeSpan.Zero);
            this.actual = this.view.SubscribeAll();
        }
    }
}