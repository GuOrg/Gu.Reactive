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
            this._view = this._ints.AsFilteredView(x => true, TimeSpan.Zero);
            this._actual = this._view.SubscribeAll();
        }
    }
}