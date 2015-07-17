namespace Gu.Reactive.Tests.Collections
{
    using System;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    public class FilteredMappedView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _scheduler = new TestScheduler();
            _view = _ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), _scheduler)
                         .AsMappingView(x => x);
            _scheduler.Start();
            _actual = _view.SubscribeAll();
        }
    }
}