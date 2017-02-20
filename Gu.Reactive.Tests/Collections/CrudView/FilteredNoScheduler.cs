namespace Gu.Reactive.Tests.Collections.CrudView
{
    using System;

    using NUnit.Framework;

    public class FilteredNoScheduler : CrudViewTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
#pragma warning disable GU0036 // Don't dispose injected.
            this.View?.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
            this.View = this.Ints.AsFilteredView(x => true, TimeSpan.Zero);
            this.ActualEventArgs = this.SubscribeAll(this.View);
        }
    }
}