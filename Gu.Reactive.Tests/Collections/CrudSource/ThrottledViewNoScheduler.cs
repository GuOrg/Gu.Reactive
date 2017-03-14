namespace Gu.Reactive.Tests.Collections
{
    using System;

    public class ThrottledViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            (this.View as IDisposable)?.Dispose();
            this.View = new ThrottledView<int>(this.Source, TimeSpan.Zero, null);
        }
    }
}