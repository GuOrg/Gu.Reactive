#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Reactive.Tests.Collections
{
    using System;

    public class ThrottledViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            (this.View as IDisposable)?.Dispose();
            this.View = this.Source.AsThrottledView(TimeSpan.Zero, null);
        }
    }
}