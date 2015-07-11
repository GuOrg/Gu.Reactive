namespace Gu.Reactive.Tests.Collections
{
    using System;

    public class ReadOnlyFilteredViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _view = new ReadOnlyFilteredView<int>(_ints, x => true, TimeSpan.Zero, null);
            _actual = SubscribeAll(_view);
        }
    }
}