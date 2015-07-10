namespace Gu.Reactive.Tests.Collections
{
    using System;

    public class DeferredViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _view = new DeferredView<int>(_ints, TimeSpan.Zero, null);
            _actual = SubscribeAll(_view);
        }
    }
}