namespace Gu.Reactive.Tests.Collections
{
    using System;

    public class ThrottledViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _view = new ThrottledView<int>(_ints, TimeSpan.Zero, null);
            _actual = SubscribeAll(_view);
        }
    }
}