namespace Gu.Reactive.Tests.Collections
{
    using System;

    public class FilteredViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            (this.View as IDisposable)?.Dispose();
            this.View = new FilteredView<int>(this.Source, x => true, TimeSpan.Zero, null);
        }
    }
}