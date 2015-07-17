namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.ObjectModel;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ReadOnlyFilteredViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            _view = new ReadOnlyFilteredView<int>(_ints, x => true, TimeSpan.Zero, null);
            _actual = _view.SubscribeAll();
        }

        [Test]
        public void InitializeFiltered()
        {
            var ints = new ObservableCollection<int>(new[] { 1, 2 });
            var view = ints.AsReadOnlyFilteredView(x => x < 2);
            CollectionAssert.AreEqual(new[] { 1 }, view);
        }
    }
}