namespace Gu.Reactive.Tests.Collections.ReadOnlyViews
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive.Concurrency;

    using Gu.Reactive.Tests.Helpers;

    using Microsoft.Reactive.Testing;

    using NUnit.Framework;

    public abstract class CrudSourceTests
    {
#pragma warning disable CS8618, SA1306
        protected VirtualTimeSchedulerBase<long, long> Scheduler;
        protected IReadOnlyObservableCollection<int> View;
        protected ObservableCollection<int> Source;
#pragma warning restore CS8618, SA1306

        [SetUp]
        public virtual void SetUp()
        {
            this.Source = new ObservableCollection<int> { 1, 2, 3 };
        }

        [Test]
        public void Ctor()
        {
            CollectionAssert.AreEqual(this.Source, this.View);
        }

        [Test]
        public void NoChangeNoEvent()
        {
            using var actual = this.View.SubscribeAll();
            CollectionAssert.AreEqual(this.Source, this.View);
            (this.View as IRefreshAble)?.Refresh();
            this.Scheduler?.Start();

            CollectionAssert.AreEqual(this.Source, this.View);
            CollectionAssert.IsEmpty(actual);
        }

        [Test]
        public void UpdatesBeforeItNotifies()
        {
            int[]? actual = null;
            int[]? expected = null;
            using (this.View.ObserveCollectionChanged(signalInitial: false)
                       .Subscribe(_ => { actual = this.View.ToArray(); }))
            {
                using (this.Source.ObserveCollectionChanged(signalInitial: false)
                           .Subscribe(_ => { expected = this.Source.ToArray(); }))
                {
                    this.Source.Add(5);
                    this.Scheduler?.Start();
                    CollectionAssert.AreEqual(expected, actual);

                    this.Source.Clear();
                    this.Scheduler?.Start();
                    CollectionAssert.AreEqual(expected, actual);
                }
            }
        }

        [Test]
        public void Add()
        {
            using var expected = this.Source.SubscribeAll();
            using var actual = this.View.SubscribeAll();
            this.Source.Add(4);
            this.Scheduler?.Start();

            CollectionAssert.AreEqual(this.Source, this.View);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [Test]
        public void ManyAddsOneReset()
        {
            if (!(this.Scheduler is TestScheduler))
            {
                Assert.Inconclusive();
            }

            using var actual = this.View.SubscribeAll();
            for (var i = 0; i < 10; i++)
            {
                this.Source.Add(i);
            }

            this.Scheduler.Start();

            CollectionAssert.AreEqual(this.Source, this.View);
            var expected = new EventArgs[]
            {
                CachedEventArgs.CountPropertyChanged,
                CachedEventArgs.IndexerPropertyChanged,
                CachedEventArgs.NotifyCollectionReset,
            };
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void Remove(int toRemove)
        {
            using var expected = this.Source.SubscribeAll();
            using var actual = this.View.SubscribeAll();
            this.Source.Remove(toRemove);
            this.Scheduler?.Start();

            CollectionAssert.AreEqual(this.Source, this.View);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [TestCase(2, 1)]
        [TestCase(0, 2)]
        public void Replace(int index, int value)
        {
            using var expected = this.Source.SubscribeAll();
            using var actual = this.View.SubscribeAll();
            this.Source[index] = value;
            this.Scheduler?.Start();

            Assert.AreEqual(value, this.View[index]);
            CollectionAssert.AreEqual(this.Source, this.View);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }

        [TestCase(0, 1)]
        public void Move(int fromIndex, int toIndex)
        {
            using var expected = this.Source.SubscribeAll();
            using var actual = this.View.SubscribeAll();
            this.Source.Move(fromIndex, toIndex);
            this.Scheduler?.Start();

            CollectionAssert.AreEqual(this.Source, this.View);
            CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
        }
    }
}
