namespace Gu.Wpf.Reactive.Tests.Collections.MutableViews.CrudView
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reactive.Concurrency;

    using Gu.Reactive;
    using Gu.Reactive.Tests.Helpers;
    using Gu.Wpf.Reactive.Tests.FakesAndHelpers;

    using NUnit.Framework;

    public abstract class CrudViewTests
    {
        protected VirtualTimeSchedulerBase<long, long> Scheduler { get; set; }

        protected IObservableCollection<int> View { get; set; }

        protected ObservableCollection<int> Ints { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            App.Start();
        }

        [SetUp]
        public virtual void SetUp()
        {
            this.Ints = new ObservableCollection<int> { 1, 2, 3 };
        }

        [TearDown]
        public void TearDown()
        {
#pragma warning disable GU0036 // Don't dispose injected.
            (this.View as IDisposable)?.Dispose();
#pragma warning restore GU0036 // Don't dispose injected.
        }

        [Test]
        public void NoChangeNoEvent()
        {
            CollectionAssert.AreEqual(this.Ints, this.View);
            using (var actual = this.View.SubscribeAll())
            {
                (this.View as IRefreshAble)?.Refresh();
                this.Scheduler?.Start();
                CollectionAssert.AreEqual(this.Ints, this.View);
                CollectionAssert.IsEmpty(actual);
            }
        }

        [Test]
        public void UpdatesBeforeItNotifies()
        {
            int[] actual = null;
            int[] expected = null;
            using (this.View.ObserveCollectionChanged(signalInitial: false)
                       .Subscribe(_ => { actual = this.View.ToArray(); }))
            {
                using (this.Ints.ObserveCollectionChanged(signalInitial: false)
                           .Subscribe(_ => { expected = this.Ints.ToArray(); }))
                {
                    this.View.Add(5);
                    this.Scheduler?.Start();
                    CollectionAssert.AreEqual(expected, actual);

                    this.View.Clear();
                    this.Scheduler?.Start();
                    CollectionAssert.AreEqual(expected, actual);
                }
            }
        }

        [Test]
        public void Add()
        {
            using (var expected = this.Ints.SubscribeAll())
            {
                using (var actual = this.View.SubscribeAll())
                {
                    this.View.Add(4);
                    this.Scheduler?.Start();
                    CollectionAssert.AreEqual(this.Ints, this.View);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void ListAdd()
        {
            using (var expected = this.Ints.SubscribeAll())
            {
                using (var actual = this.View.SubscribeAll())
                {
                    // DataGrid adds items like this
                    var index = ((IList)this.View).Add(4);
                    this.Scheduler?.Start();
                    Assert.AreEqual(3, index);
                    CollectionAssert.AreEqual(this.Ints, this.View);
                    CollectionAssert.IsNotEmpty(actual);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);

                    var before = actual.ToArray();
                    this.Scheduler?.Start(); // Should not signal deferred

                    CollectionAssert.AreEqual(before, actual, EventArgsComparer.Default);
                }
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        public void Remove(int toRemove)
        {
            using (var expected = this.Ints.SubscribeAll())
            {
                using (var actual = this.View.SubscribeAll())
                {
                    this.View.Remove(toRemove);
                    this.Scheduler?.Start();
                    CollectionAssert.AreEqual(this.Ints, this.View);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [TestCase(2, 5)]
        [TestCase(0, 5)]
        public void ReplaceIndexer(int index, int value)
        {
            using (var expected = this.Ints.SubscribeAll())
            {
                using (var actual = this.View.SubscribeAll())
                {
                    this.View[index] = value;
                    this.Scheduler?.Start();
                    Assert.AreEqual(value, this.View[index]);
                    CollectionAssert.AreEqual(this.Ints, this.View);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [TestCase(0, 1)]
        [TestCase(0, 2)]
        [TestCase(1, 2)]
        [TestCase(2, 1)]
        [TestCase(1, 0)]
        [TestCase(2, 0)]
        public void Move(int fromIndex, int toIndex)
        {
            using (var expected = this.Ints.SubscribeAll())
            {
                using (var actual = this.View.SubscribeAll())
                {
                    this.View.Move(fromIndex, toIndex);
                    this.Scheduler?.Start();
                    CollectionAssert.AreEqual(this.Ints, this.View);
                    CollectionAssert.AreEqual(expected, actual, EventArgsComparer.Default);
                }
            }
        }

        [Test]
        public void Count()
        {
            Assert.AreEqual(3, this.View.Count);
        }

        [Test]
        public void ToArrayTest()
        {
            CollectionAssert.AreEqual(this.Ints, this.View.ToArray());
        }
    }
}