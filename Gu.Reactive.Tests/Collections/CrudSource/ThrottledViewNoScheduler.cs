namespace Gu.Reactive.Tests.Collections
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    public class ThrottledViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.View = new ThrottledView<int>(this.Ints, TimeSpan.Zero, null);
            this.Actual = this.View.SubscribeAll();
        }
    }
}