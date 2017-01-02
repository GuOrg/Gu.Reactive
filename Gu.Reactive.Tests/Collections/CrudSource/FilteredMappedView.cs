namespace Gu.Reactive.Tests.Collections
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    public class FilteredMappedView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
            this.View = this.Ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), this.Scheduler)
                         .AsMappingView(x => x);
            this.Scheduler.Start();
            this.Actual = this.View.SubscribeAll();
        }
    }
}