namespace Gu.Reactive.Tests.Collections.Filter
{
    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class FilteredView : FilterTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this._scheduler = new TestScheduler();

            this._view = this._ints.AsFilteredView(x => true, this._scheduler);
            this._actual = this._view.SubscribeAll();
        }
    }
}