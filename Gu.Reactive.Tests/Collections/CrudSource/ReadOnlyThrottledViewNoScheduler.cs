namespace Gu.Reactive.Tests.Collections
{
    using System;

    public class ReadOnlyThrottledViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _view = new ReadOnlyThrottledView<int>(_ints, TimeSpan.Zero, null);
            _actual = SubscribeAll(_view);
        }
    }
}