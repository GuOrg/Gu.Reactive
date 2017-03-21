namespace Gu.Reactive.Tests.Collections.ReadOnlyViews
{
    using System;

    public class ReadOnlyThrottledViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            (this.View as IDisposable)?.Dispose();
            this.View = new ReadOnlyThrottledView<int>(this.Source, TimeSpan.Zero, null, false);
        }
    }
}