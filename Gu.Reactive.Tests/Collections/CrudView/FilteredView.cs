namespace Gu.Reactive.Tests.Collections.CrudView
{
    using System;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class FilteredView : CrudViewTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
            this.View?.Dispose();
            this.View = this.Ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), this.Scheduler);
            this.Actual = this.SubscribeAll(this.View);
        }
    }
}