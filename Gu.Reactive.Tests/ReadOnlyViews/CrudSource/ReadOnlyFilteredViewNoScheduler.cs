namespace Gu.Reactive.Tests.ReadOnlyViews
{
    using System;
    using System.Collections.ObjectModel;
    using NUnit.Framework;

    public class ReadOnlyFilteredViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
#pragma warning disable IDISP007 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            this.View = new ReadOnlyFilteredView<int>(this.Source, x => true, TimeSpan.Zero, null);
        }

        [Test]
        public void InitializeFiltered()
        {
            var ints = new ObservableCollection<int> { 1, 2 };
            using var view = ints.AsReadOnlyFilteredView(x => x < 2);
            CollectionAssert.AreEqual(new[] { 1 }, view);
        }
    }
}