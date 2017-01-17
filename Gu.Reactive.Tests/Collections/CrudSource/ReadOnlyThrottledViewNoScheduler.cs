namespace Gu.Reactive.Tests.Collections
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    public class ReadOnlyThrottledViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            (this.View as IDisposable)?.Dispose();
            this.View = new ReadOnlyThrottledView<int>(this.Ints, TimeSpan.Zero, null);
            this.Actual = this.View.SubscribeAll();
        }
    }
}