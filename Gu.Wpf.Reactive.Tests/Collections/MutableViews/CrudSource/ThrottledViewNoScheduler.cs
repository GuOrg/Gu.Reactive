#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews.CrudSource
{
    using System;

    using Gu.Reactive.Tests.Collections.ReadOnlyViews;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using NUnit.Framework;

    public class ThrottledViewNoScheduler : CrudSourceTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            App.Start();
        }

        public override void SetUp()
        {
            this.Scheduler = new TestDispatcherScheduler();
            base.SetUp();
#pragma warning disable IDISP007 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            this.View = this.Source.AsThrottledView(TimeSpan.Zero);
        }
    }
}