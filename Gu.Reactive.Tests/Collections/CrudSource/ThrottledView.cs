#pragma warning disable CS0618 // Type or member is obsolete
namespace Gu.Reactive.Tests.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using Gu.Reactive.Tests.Helpers;
    using Microsoft.Reactive.Testing;
    using NUnit.Framework;

    public class ThrottledView : CrudSourceTests
    {
        public override void SetUp()
        {
            base.SetUp();
            this.Scheduler = new TestScheduler();
            (this.View as IDisposable)?.Dispose();
            this.View = this.Source.AsThrottledView(TimeSpan.FromMilliseconds(10), this.Scheduler);
            this.Scheduler.Start();
        }
    }
}