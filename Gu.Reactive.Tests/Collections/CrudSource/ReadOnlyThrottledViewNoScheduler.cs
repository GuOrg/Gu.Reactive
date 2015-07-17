namespace Gu.Reactive.Tests.Collections
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    public class ReadOnlyThrottledViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _view = new ReadOnlyThrottledView<int>(_ints, TimeSpan.Zero, null);
            _actual = _view.SubscribeAll();
        }
    }
}