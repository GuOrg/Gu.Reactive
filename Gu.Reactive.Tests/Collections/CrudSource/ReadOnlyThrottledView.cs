namespace Gu.Reactive.Tests.Collections
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    public class ReadOnlyThrottledView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _scheduler = new TestScheduler();
            _view = new ReadOnlyThrottledView<int>(_ints, TimeSpan.FromMilliseconds(10), _scheduler);
            _scheduler.Start();
            _actual = _view.SubscribeAll();
        }
    }
}