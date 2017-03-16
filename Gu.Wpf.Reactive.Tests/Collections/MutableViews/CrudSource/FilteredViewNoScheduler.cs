#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews.CrudSource
{
    using System;

    using Gu.Reactive.Tests.Collections;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;

    public class FilteredViewNoScheduler : CrudSourceTests
    {
        public override void SetUp()
        {
            App.Start();
            this.Scheduler = new TestDispatcherScheduler();
            base.SetUp();
            (this.View as IDisposable)?.Dispose();
            this.View = new FilteredView<int>(this.Source, x => true, TimeSpan.Zero, null);
        }
    }
}