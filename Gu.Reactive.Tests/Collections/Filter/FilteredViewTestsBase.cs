namespace Gu.Reactive.Tests.Collections.Filter
{
    using Gu.Reactive.Tests.Helpers;

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
            this.view = this.ints.AsFilteredView(x => true, this.scheduler);
        }
    }
}