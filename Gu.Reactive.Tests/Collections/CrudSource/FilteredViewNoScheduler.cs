namespace Gu.Reactive.Tests.Collections
{
    using System;

    public class FilteredViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _view = new FilteredView<int>(_ints, x => true, TimeSpan.Zero, null);
            _actual = SubscribeAll(_view);
        }
    }
}