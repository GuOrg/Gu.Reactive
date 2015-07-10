namespace Gu.Reactive.Tests.Collections
{
    using System;

    public class ReadOnlyDeferredViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _view = new ReadOnlyDeferredView<int>(_ints, TimeSpan.Zero, null);
            _actual = SubscribeAll(_view);
        }
    }
}