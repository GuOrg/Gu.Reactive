namespace Gu.Reactive.Tests.Collections.CrudView
{
    using System;

    using NUnit.Framework;

    public class FilteredNoScheduler : CrudViewTests
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            _view = _ints.AsFilteredView(x => true, TimeSpan.Zero);
            _actual = SubscribeAll(_view);
        }
    }
}