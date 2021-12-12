namespace Gu.Reactive.Tests.ReadOnlyViews
{
    using System;

    public class ReadOnlyThrottledViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
#pragma warning disable IDISP007 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            this.View = new ReadOnlyThrottledView<int>(this.Source, TimeSpan.Zero, null, leaveOpen: false);
        }
    }
}
