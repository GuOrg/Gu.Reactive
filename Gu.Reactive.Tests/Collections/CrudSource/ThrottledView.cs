namespace Gu.Reactive.Tests.Collections
{
    using System;

    using Microsoft.Reactive.Testing;

    public class ThrottledView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _scheduler = new TestScheduler();
            _view = new ThrottledView<int>(_ints, TimeSpan.FromMilliseconds(10), _scheduler);
            _scheduler.Start();
            _actual = SubscribeAll(_view);
        }
    }
}