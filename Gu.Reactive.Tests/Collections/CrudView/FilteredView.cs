namespace Gu.Reactive.Tests.Collections.CrudView
{
    using System;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public class FilteredView : CrudViewTests
    {
        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            _scheduler = new TestScheduler();

            _view = _ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10), _scheduler);
            _actual = SubscribeAll(_view);
        }
    }
}