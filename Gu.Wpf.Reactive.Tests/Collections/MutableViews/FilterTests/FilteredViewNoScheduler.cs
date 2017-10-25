#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews.FilterTests
{
    using System;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;
    using NUnit.Framework;

    public class FilteredViewNoScheduler : FilterTests
    {
        [SetUp]
        public override void SetUp()
        {
            App.Start();
            this.Scheduler = new TestDispatcherScheduler();
            base.SetUp();
#pragma warning disable IDISP007 // Don't dispose injected.
            this.View?.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected.
            this.View = this.Source.AsFilteredView(x => true, TimeSpan.Zero);
        }
    }
}