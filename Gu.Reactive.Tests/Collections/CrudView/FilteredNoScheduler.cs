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
            this.View?.Dispose();
            this.View = this.Ints.AsFilteredView(x => true, TimeSpan.Zero);
            this.ActualEventArgs = this.SubscribeAll(this.View);
        }
    }
}