namespace Gu.Reactive.Tests.Collections.CrudView
{
    using System;

    using NUnit.Framework;

    public class FilteredNoScheduler : CrudViewTests
    {
        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            _view = _ints.AsFilteredView(x => true, TimeSpan.FromMilliseconds(10));
            _actual = SubscribeAll(_view);
        }
    }
}