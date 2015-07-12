namespace Gu.Reactive.Tests.Collections.Filter
{
    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class FilteredView : FilterTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _scheduler = new TestScheduler();

            _view = _ints.AsFilteredView(x => true, _scheduler);
            _actual = _view.SubscribeAll();
        }
    }
}