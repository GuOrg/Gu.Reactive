namespace Gu.Reactive.Tests.Collections.CrudView
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using Gu.Reactive.Tests.Helpers;

    using NUnit.Framework;

    public class ThrottledViewTests
    {
        private ObservableCollection<int> source;
        private TimeSpan deferTime;
        private IThrottledView<int> throttledView;
        private readonly List<NotifyCollectionChangedEventArgs> changes = new List<NotifyCollectionChangedEventArgs>();

        [SetUp]
        public void SetUp()
        {
            this.source = new ObservableCollection<int>(new[] { 1, 2, 3 });
            this.deferTime = TimeSpan.FromMilliseconds(10);
            this.throttledView = this.source.AsThrottledView(this.deferTime);
            this.changes.Clear();
        }

        [Test]
        public void OneChangeOneNotification()
        {
            this.throttledView.CollectionChanged += (_, e) => this.changes.Add(e);
            this.source.Add(4);
            this.throttledView.Refresh();
            CollectionAssert.AreEqual(this.source, this.throttledView);
            var expected = new[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 4, 3) };
            CollectionAssert.AreEqual(expected, this.changes, EventArgsComparer.Default);
        }

        [Test]
        public void ManyChangeOneReset()
        {
            this.throttledView.CollectionChanged += (_, e) => this.changes.Add(e);
            for (int i = 0; i < 10; i++)
            {
                this.source.Add(i);
            }

            this.throttledView.Refresh();
            CollectionAssert.AreEqual(this.source, this.throttledView);
            var expected = new[] { new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset) };
            CollectionAssert.AreEqual(expected, this.changes, EventArgsComparer.Default);
        }

        [Test]
        public void TwoBurstsTwoResets()
        {
            this.throttledView.CollectionChanged += (_, e) => this.changes.Add(e);
            for (int i = 0; i < 10; i++)
            {
                this.source.Add(i);
            }

            this.throttledView.Refresh();
            for (int i = 0; i < 10; i++)
            {
                this.source.Add(i);
            }

            this.throttledView.Refresh();
            CollectionAssert.AreEqual(this.source, this.throttledView);
            var expected = new[]
            {
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
            };
            CollectionAssert.AreEqual(expected, this.changes, EventArgsComparer.Default);
        }
    }
}
