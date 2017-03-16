#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews.CrudView
{
    using System;

    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;

    using NUnit.Framework;

    public class FilteredViewNoThrottling : CrudViewTests
    {
        [SetUp]
        public override void SetUp()
        {
            App.Start();
            base.SetUp();
#pragma warning disable GU0036 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
            this.View = this.Ints.AsFilteredView(x => true, TimeSpan.Zero);
        }
    }
}