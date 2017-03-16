namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews.CrudView
{
    using System;

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
#pragma warning disable GU0036 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
            this.View = new DispatchingView<int>(this.Ints);
        }
    }
}