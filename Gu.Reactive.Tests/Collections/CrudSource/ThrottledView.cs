namespace Gu.Reactive.Tests.Collections
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    public class ThrottledView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
            (this.View as IDisposable)?.Dispose();
            this.View = new ThrottledView<int>(this.Ints, TimeSpan.FromMilliseconds(10), this.Scheduler);
            this.Scheduler.Start();
            this.ActualEventArgs?.Dispose();
            this.ActualEventArgs = this.View.SubscribeAll();
        }
    }
}