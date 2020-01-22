namespace Gu.Wpf.Reactive.Tests.Collections.Views.CrudView
{
    using System;
    using Gu.Reactive.Tests.Helpers;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;

    public class DispatchingView : CrudViewTests
    {
        [SetUp]
        public override void SetUp()
        {
            App.Start();
            base.SetUp();
            this.Scheduler = new TestScheduler();
#pragma warning disable IDISP007 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            this.View = new DispatchingView<int>(this.Ints, TimeSpan.Zero, leaveOpen: false);
        }

        [Test]
        public void AddFour()
        {
            using var expected = this.Ints.SubscribeAll();
            using var actual = this.View.SubscribeAll();
            for (var i = 0; i < 4; i++)
            {
                this.View.Add(i);
            }

            this.Scheduler?.Start();
            CollectionAssert.AreEqual(this.Ints, this.View);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }
    }
}
