namespace Gu.Reactive.Tests.Collections
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    public class FilteredViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.View = new FilteredView<int>(this.Ints, x => true, TimeSpan.Zero, null);
            this.Actual = this.View.SubscribeAll();
        }
    }
}